using Utilities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Reflection;
using System.Text.Json.Serialization;
using Models.ExcelModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services.Abstract;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        readonly IExcelService _excelService;

        public ExcelController(IExcelService excelService)
        {
            _excelService = excelService;
        }

        [HttpPost("Export/{key}")]
        public async Task<IActionResult> ExportAsync([FromRoute] string key, [FromBody]ExportDataQueryModel query)
        {
            var file = await _excelService.ExportAsync(key, query);

            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{key} List.xlsx");
        }

        [HttpPost("Import/{key}")]
        public async Task<IActionResult> ImportAsync([FromRoute] string key, IFormFile file)
        {
            await _excelService.ImportAsync(key, file);

            return Ok();
        }

        
    }
}
