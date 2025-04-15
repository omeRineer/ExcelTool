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
using System.Xml.Linq;
using Core.Utilities.Helpers;
using MeArchitecture.Reporting.Attributes;
using MeArchitecture.Reporting.Data;
using MeArchitecture.Reporting.Adapters;
using MeArchitecture.Reporting.Models.Schemas;
using Services.DataBase.Context;
using Microsoft.AspNetCore.Http;
using Models.Query;

namespace Services.Concrete
{
    public interface IReportService
    {
        Task<Stream> ExportAsync(string key, DynamicDataQueryModel queryModel);
        Task ImportAsync(string key, IFormFile file);
    }
    public class ReportService : IReportService
    {
        readonly IReportAdapter _excelAdapter;
        readonly IReportSchemaService _reportSchemaService;
        public ReportService(IReportAdapter excelAdapter, IReportSchemaService reportSchemaService)
        {
            _excelAdapter = excelAdapter;
            _reportSchemaService = reportSchemaService;
        }

        public async Task ImportAsync(string key, IFormFile file)
        {
            var schemaOpt = await GetSchemaAsync<ExcelImportSchema>(key);
            Type type = GetDataType(schemaOpt.Entity.Object);

            var excelProperties = ReflectionHelper.GetProperties(type, attributeTypes: new Type[] { typeof(ReportProperty) });
            var dataCollection = await _excelAdapter.ReadToDictionaryAsync(file.OpenReadStream());

            foreach (var row in dataCollection)
            {
                var instance = Activator.CreateInstance(type);

                foreach (var column in schemaOpt.Schema.Columns.Where(f => excelProperties.Select(s => s.FullName).Contains(f.Property)))
                {
                    var value = ChangeType(row[column.Name], column.Type);

                    ReflectionHelper.SetPropertyValue(instance, column.Property, value);
                }

                await _reportSchemaService.AddAsync(instance);
            }

            await _reportSchemaService.SaveAsync();
        }

        public async Task<Stream> ExportAsync(string key, DynamicDataQueryModel queryModel)
        {
            var schemaOpt = await GetSchemaAsync<ExcelExportSchema>(key);
            Type type = GetDataType(schemaOpt.Entity.Object);

            var excelProperties = ReflectionHelper.GetProperties(type, attributeTypes: new Type[] { typeof(ReportProperty) });
            var selectedColumns = schemaOpt.Schema?.Columns?.Where(f => excelProperties.Select(s => s.FullName).Contains(f.Property)).Select(s => new KeyValuePair<string, string>(s.Name, s.Property)).ToList();
            var data = await _reportSchemaService.GetDataWithDynamicAsync(type, schemaOpt.Schema, queryModel);
            var excelStream = await _excelAdapter.CreateAsync(selectedColumns,
                                                   data);

            return excelStream;
        }

        #region Private
        private async Task<(ReportSchema Entity, TSchema Schema)> GetSchemaAsync<TSchema>(string key)
        {
            ReportSchema? schemaEntity = await _reportSchemaService.GetExcelSchemaAsync(key);

            if (schemaEntity == null)
                throw new ArgumentNullException(nameof(key));

            TSchema? schema = JsonConvert.DeserializeObject<TSchema>(schemaEntity.Schema);

            return (schemaEntity, schema);
        }
        private Type GetDataType(string name)
        {
            Type? type = Assembly.Load("Entities")
                                 .GetTypes()
                                 .Where(f => f.GetCustomAttributes(typeof(ReportObject), false).Length > 0)
                                 .Single(f => f.GetCustomAttribute<ReportObject>()?.Title == name || f.Name == name);

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type;
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
