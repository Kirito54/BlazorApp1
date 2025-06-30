using BlazorApp1.Services;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ExcelDataService _excel;

    public SearchController(ExcelDataService excel)
    {
        _excel = excel;
    }

    [HttpGet("count")]
    public ActionResult<int> GetCount()
    {
        return _excel.GetTotalCount();
    }

    [HttpGet]
    public ActionResult<SearchResponse> Search([FromQuery] string? orderNumber, [FromQuery] string? mfcNumber)
    {
        var records = _excel.Search(orderNumber, mfcNumber).ToList();
        var max = _excel.Records
            .Select(r => int.TryParse(r.QueueNumber, out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return new SearchResponse
        {
            Records = records,
            TotalCount = max
        };
    }
}
