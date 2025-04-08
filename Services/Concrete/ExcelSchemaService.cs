using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Models.ExcelModels;
using Models.ExcelSchemas;
using Newtonsoft.Json;
using OfficeOpenXml;
using Services.Abstract;
using Services.DataBase.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class ExcelSchemaService : IExcelSchemaService
    {
        readonly CoreContext Context;
        public ExcelSchemaService(CoreContext context)
        {
            Context = context;
        }

        public async Task AddAsync(object data)
        {
            await Context.AddAsync(data);
        }

        public async Task<List<object>> GetDataWithDynamicAsync(Type type, ExcelExportSchema excelExportShema, ExportDataQueryModel query)
        {
            var context = Context.GetType().GetMethods()
                                 .Where(f => f.Name == "GetData" && f.IsGenericMethod)
                                 .Single()
                                 .MakeGenericMethod(type);

            var args = new List<object>();
            if (query != null)
                args.Add(query);
            if (excelExportShema.Includes != null || excelExportShema.Includes?.Length > 0)
                args.Add(excelExportShema.Includes.ToArray());

            var table = (IQueryable<object>)context.Invoke(Context, args.ToArray());

            var data = table?.ToList();

            return data;
        }

        public async Task<ExcelSchema> GetExcelSchemaAsync(string key)
        {
            var schemaData = await Context.Set<ExcelSchema>().SingleAsync(f => f.Key == key);

            return schemaData;
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
