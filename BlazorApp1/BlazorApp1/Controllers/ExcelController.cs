using BlazorApp1.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExcelController : ControllerBase
{
    private readonly ExcelDataService _excel;

    public ExcelController(ExcelDataService excel)
    {
        _excel = excel;
    }

    [HttpPost("validate-excel")]
    public async Task<IActionResult> Validate([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File not provided");
        }
        using var stream = file.OpenReadStream();
        var ok = await _excel.LoadAsync(stream);
        if (!ok)
        {
            return BadRequest("Ошибка: файл содержит персональные данные");
        }
        return Ok();
    }
}
