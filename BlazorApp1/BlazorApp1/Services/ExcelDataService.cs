using System.Text.RegularExpressions;
using BlazorApp1.Models;
using ClosedXML.Excel;

namespace BlazorApp1.Services;

public class ExcelDataService
{
    private readonly ILogger<ExcelDataService> _logger;
    private readonly List<QueueInfo> _records = new();

    public ExcelDataService(ILogger<ExcelDataService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<QueueInfo> Records => _records;

    public async Task<bool> LoadAsync(Stream stream)
    {
        _records.Clear();
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
            _records.Add(new QueueInfo
            {
                QueueNumber = queue,
                MfcNumber = mfc,
                OrderNumber = order
            });
        }
        return true;
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
            matches = matches.Where(r => r.OrderNumber.Contains(orderNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(mfcNumber))
        {
            matches = matches.Where(r => r.MfcNumber.Contains(mfcNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (string.IsNullOrWhiteSpace(orderNumber) && string.IsNullOrWhiteSpace(mfcNumber))
        {
            return Enumerable.Empty<QueueInfo>();
        }

        return matches.ToList();
    }
}
