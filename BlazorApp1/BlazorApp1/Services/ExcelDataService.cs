using System.Text.RegularExpressions;
using System.IO;
using BlazorApp1.Models;
using ClosedXML.Excel;

namespace BlazorApp1.Services;

public class ExcelDataService
{
    private readonly ILogger<ExcelDataService> _logger;
    private readonly List<QueueInfo> _records = new();
    private const string SavedFile = "data/latest.xlsx";

    public ExcelDataService(ILogger<ExcelDataService> logger)
    {
        _logger = logger;
        TryLoadFromDisk();
    }

    public IReadOnlyList<QueueInfo> Records => _records;

    public async Task<bool> LoadAsync(Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;

        var newRecords = new List<QueueInfo>();
        if (!LoadFromStream(ms, newRecords))
        {
            return false;
        }
        _records.Clear();
        _records.AddRange(newRecords);

        Directory.CreateDirectory(Path.GetDirectoryName(SavedFile)!);
        ms.Position = 0;
        using var fs = new FileStream(SavedFile, FileMode.Create, FileAccess.Write, FileShare.None);
        await ms.CopyToAsync(fs);

        return true;
    }

    private bool LoadFromStream(Stream stream, List<QueueInfo> target)
    {
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheets.First();
        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var queue = row.Cell(1).GetString();
            var mfc = row.Cell(2).GetString();
            var order = row.Cell(3).GetString();
            if (ContainsPersonalData(queue) || ContainsPersonalData(mfc) || ContainsPersonalData(order))
            {
                return false;
            }
            target.Add(new QueueInfo
            {
                QueueNumber = queue,
                MfcNumber = mfc,
                OrderNumber = order
            });
        }
        return true;
    }

    private void TryLoadFromDisk()
    {
        if (!File.Exists(SavedFile))
            return;
        try
        {
            using var fs = new FileStream(SavedFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            var newRecords = new List<QueueInfo>();
            if (LoadFromStream(fs, newRecords))
            {
                _records.Clear();
                _records.AddRange(newRecords);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Excel data from disk");
        }
    }

    private static readonly Regex[] PersonalDataPatterns =
    {
        new(@"\b\d{3}-\d{3}-\d{3} \d{2}\b", RegexOptions.Compiled), // SNILS
        new(@"\b\d{11}\b", RegexOptions.Compiled), // INN etc
        new(@"(?i)паспорт", RegexOptions.Compiled),
        new(@"[А-ЯЁ][а-яё]+\s+[А-ЯЁ][а-яё]+\s+[А-ЯЁ][а-яё]+", RegexOptions.Compiled)
    };

    private static bool ContainsPersonalData(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return PersonalDataPatterns.Any(p => p.IsMatch(value));
    }

    public IEnumerable<QueueInfo> Search(string? orderNumber, string? mfcNumber)
    {
        IEnumerable<QueueInfo> matches = _records;

        if (!string.IsNullOrWhiteSpace(orderNumber))
        {
            matches = matches.Where(r => string.Equals(r.OrderNumber, orderNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(mfcNumber))
        {
            matches = matches.Where(r => string.Equals(r.MfcNumber, mfcNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (string.IsNullOrWhiteSpace(orderNumber) && string.IsNullOrWhiteSpace(mfcNumber))
        {
            return Enumerable.Empty<QueueInfo>();
        }

        return matches.ToList();
    }
}
