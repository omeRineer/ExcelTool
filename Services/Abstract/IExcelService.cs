using Microsoft.AspNetCore.Http;
using Models.ExcelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface IExcelService
    {
        Task<ExcelObjectListModel> GetExcelTypesAsync();
        Task<ExcelPropertyListModel> GetExcelPropertiesAsync(string type);
        Task<Stream> ExportAsync(string key, ExportDataQueryModel query);
        Task ImportAsync(string key, IFormFile file);
    }
}
