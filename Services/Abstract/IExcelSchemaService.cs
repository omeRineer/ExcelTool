using Entities.Concrete;
using Models.ExcelModels;
using Models.ExcelSchemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract
{
    public interface IExcelSchemaService
    {
        Task<List<object>> GetDataWithDynamicAsync(Type type, ExcelExportSchema excelExportShema, ExportDataQueryModel query);
        Task<ExcelSchema> GetExcelSchemaAsync(string key);
        Task AddAsync(object data);
        Task SaveAsync();
    }
}
