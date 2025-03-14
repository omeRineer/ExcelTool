using Utilities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using Services.Concrete;
using System.Reflection;
using System.Text.Json.Serialization;
using Models.ExcelModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        [HttpPost("ExportToFile/{entityType}")]
        public async Task<IActionResult> ExportToFile([FromRoute] string entityType, [FromBody]ExportDataQueryModel query)
        {
            var file = _excelService.Export(entityType, query);

            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{entityType} List.xlsx");
        }

        [HttpPost("ImportFile/{entityType}")]
        public async Task<IActionResult> ImportFile([FromRoute] string entityType, IFormFile file)
        {
            _excelService.Import(entityType, file);

            return Ok();
        }

        
    }
}
