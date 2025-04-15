using MeArchitecture.Reporting.Data;
using MeArchitecture.Reporting.Models.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.Query;
using Newtonsoft.Json;
using OfficeOpenXml;
using Services.DataBase.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public interface IReportSchemaService
    {
        Task<List<object>> GetDataWithDynamicAsync(Type type, ExcelExportSchema excelExportShema, DynamicDataQueryModel query);
        Task<ReportSchema> GetExcelSchemaAsync(string key);
        Task<List<ReportSchema>?> GetExcelSchemaListAsync();
        Task AddAsync(object data);
        Task SaveAsync();
    }
    public class ReportSchemaService: IReportSchemaService
    {
        readonly CoreContext Context;
        public ReportSchemaService(CoreContext context)
        {
            Context = context;
        }

        public async Task AddAsync(object data)
        {
            await Context.AddAsync(data);
        }

        public async Task<List<object>> GetDataWithDynamicAsync(Type type, ExcelExportSchema excelExportShema, DynamicDataQueryModel query)
        {
            var context = Context.GetType().GetMethods()
                                 .Where(f => f.Name == "GetDynamicQuery" && f.IsGenericMethod)
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

        public async Task<ReportSchema?> GetExcelSchemaAsync(string key)
        {
            var schemaData = await Context.Set<ReportSchema>().SingleAsync(f => f.Key == key);

            return schemaData;
        }

        public async Task<List<ReportSchema>?> GetExcelSchemaListAsync()
        {
            var schemaList = await Context.Set<ReportSchema>().ToListAsync();

            return schemaList;
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
