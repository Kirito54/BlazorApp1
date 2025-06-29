using BlazorApp1.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public IActionResult Search([FromQuery] string orderNumber, [FromQuery] string? mfcNumber)
    {
        var records = _excel.Search(orderNumber, mfcNumber);
        return Ok(records);
    }
}
