using Utilities.Helpers;
using Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Models.ExcelSchemas;
using Services.DataBase.Context;
using Entities.Concrete;
using Utilities.Helpers;
using Models.ExcelModels;
using Utilities.Extensions;
using Entities;
using Services.Abstract;
using Adapters;

namespace Services.Concrete
{
    public class ExcelService : IExcelService
    {
        readonly IExcelSchemaService _excelSchemaService;
        readonly IExcelAdapter _excelAdapter;
        public ExcelService(IExcelSchemaService excelSchemaService, IExcelAdapter excelAdapter)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _excelSchemaService = excelSchemaService;
            _excelAdapter = excelAdapter;
        }

        public async Task ImportAsync(string key, IFormFile file)
        {
            var excelSchema = await _excelSchemaService.GetExcelSchemaAsync(key);
            var schema = JsonConvert.DeserializeObject<ExcelImportSchema>(excelSchema.Object);
            var type = GetDataType(excelSchema.Object);
            var dataCollection = _excelAdapter.ConvertToDictionary(file.OpenReadStream());
            

            foreach (var row in dataCollection)
            {
                var instance = (ExcelSchema) Activator.CreateInstance(type);

                foreach (var column in schema.Columns)
                {
                    var value = ChangeType(row[column.Name], column.Type);

                    ReflectionHelper.SetPropertyValue(instance, column.Property, value);
                }

                await _excelSchemaService.AddAsync(instance);
            }

            await _excelSchemaService.SaveAsync();
        }

        public async Task<Stream> ExportAsync(string key, ExportDataQueryModel query)
        {
            var excelSchema = await _excelSchemaService.GetExcelSchemaAsync(key);
            var schema = JsonConvert.DeserializeObject<ExcelExportSchema>(excelSchema.Object);
            var type = GetDataType(excelSchema.Object);
            var data = await _excelSchemaService.GetDataWithDynamicAsync(type, schema, query);

            var excelStream = _excelAdapter.Create(schema.Columns?.Select(s => new KeyValuePair<string, string>(s.Name, s.Property)).ToList(),
                                                   data);

            return excelStream;
        }

        #region Private
        
        private Type GetDataType(string name)
        {
            var dataType = Assembly.Load("Entities")
                                    .GetTypes()
                                    .Where(f => f.GetCustomAttributes(typeof(ExcelObject), false).Length > 0)
                                    .Single(f => f.GetCustomAttribute<ExcelObject>()?.Title == name);

            return dataType;
        }
        private object? ChangeType(object value, string type)
        {
            switch (type)
            {
                case "Guid":
                    return Guid.Parse(value.ToString());

                default:
                    return Convert.ChangeType(value, Enum.Parse<TypeCode>(type));
            }
        }

        #endregion
    }
}
