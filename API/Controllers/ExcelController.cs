using Utilities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.Query;
using Services.Concrete;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        readonly IReportService _excelService;

        public ExcelController(IReportService excelService)
        {
            _excelService = excelService;
        }

        //[HttpGet("Types")]
        //public async Task<IActionResult> Types()
        //{
        //    var excelObjects = await _excelService.GetExcelTypesAsync();

        //    return Ok(excelObjects);
        //}

        //[HttpGet("Properties/{type}")]
        //public async Task<IActionResult> Properties(string type)
        //{
        //    var typeProperties = await _excelService.GetExcelPropertiesAsync(type);

        //    return Ok(typeProperties);
        //}

        [HttpPost("Export/{key}")]
        public async Task<IActionResult> ExportAsync([FromRoute] string key, [FromBody]DynamicDataQueryModel query)
        {
            var file = await _excelService.ExportAsync(key, query);

            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{key}.xlsx");
        }

        [HttpPost("Import/{key}")]
        public async Task<IActionResult> ImportAsync([FromRoute] string key, IFormFile file)
        {
            await _excelService.ImportAsync(key, file);

            return Ok();
        }

        
    }
}
