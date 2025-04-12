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
using System.Xml.Linq;

namespace Services.Concrete
{
    public class ExcelService : IExcelService
    {
        readonly IExcelSchemaService _excelSchemaService;
        readonly IExcelAdapter _excelAdapter;
        public ExcelService(IExcelSchemaService excelSchemaService, IExcelAdapter excelAdapter)
        {
            _excelSchemaService = excelSchemaService;
            _excelAdapter = excelAdapter;
        }

        public async Task<ExcelObjectListModel> GetExcelTypesAsync()
        {
            var schemas = await _excelSchemaService.GetExcelSchemaListAsync();
            IEnumerable<Type> types = Assembly.Load("Entities")
                                              .GetTypes()
                                              .Where(f => f.GetCustomAttribute<ExcelObject>() != null);

            var result = new ExcelObjectListModel
            {
                Objects = types.Select(s =>
                {
                    var title = s.GetCustomAttribute<ExcelObject>()?.Title ?? s.Name;

                    return new ExcelObjectList_Item
                    {
                        Title = title,
                        Schemas = schemas?.Where(s => s.Object == title)
                                          .Select(s => s.Key)
                                          .ToArray()
                    };

                }).ToList()
            };

            return result;
        }

        public async Task<ExcelPropertyListModel> GetExcelPropertiesAsync(string objectType)
        {
            Type? type = GetDataType(objectType);
            var properties = ReflectionHelper.GetProperties(type, attributeTypes: new Type[] { typeof(ExcelProperty) });

            var result = new ExcelPropertyListModel
            {
                Properties = properties.Select(s => new ExcelPropertyList_Item
                {
                    Title = s.FullName,
                    Type = s.Info.PropertyType.Name
                }).ToList()
            };

            return result;
        }

        public async Task ImportAsync(string key, IFormFile file)
        {
            var schemaOpt = await GetSchemaAsync<ExcelImportSchema>(key);
            Type type = GetDataType(schemaOpt.Entity.Object);

            var excelProperties = ReflectionHelper.GetProperties(type, attributeTypes: new Type[] { typeof(ExcelProperty) });
            var dataCollection = await _excelAdapter.ReadToDictionaryAsync(file.OpenReadStream());


            foreach (var row in dataCollection)
            {

                var instance = Activator.CreateInstance(type);

                foreach (var column in schemaOpt.Schema.Columns.Where(f => excelProperties.Select(s => s.FullName).Contains(f.Property)))
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
            var schemaOpt = await GetSchemaAsync<ExcelExportSchema>(key);
            Type type = GetDataType(schemaOpt.Entity.Object);

            var excelProperties = ReflectionHelper.GetProperties(type, attributeTypes: new Type[] { typeof(ExcelProperty) });
            var selectedColumns = schemaOpt.Schema?.Columns?.Where(f => excelProperties.Select(s => s.FullName).Contains(f.Property)).Select(s => new KeyValuePair<string, string>(s.Name, s.Property)).ToList();


            var data = await _excelSchemaService.GetDataWithDynamicAsync(type, schemaOpt.Schema, query);
            var excelStream = await _excelAdapter.CreateAsync(selectedColumns,
                                                   data);

            return excelStream;
        }

        #region Private
        private async Task<(ExcelSchema Entity, TSchema Schema)> GetSchemaAsync<TSchema>(string key)
        {
            ExcelSchema? schemaEntity = await _excelSchemaService.GetExcelSchemaAsync(key);

            if (schemaEntity == null)
                throw new ArgumentNullException(nameof(key));

            TSchema? schema = JsonConvert.DeserializeObject<TSchema>(schemaEntity.Schema);

            return (schemaEntity, schema);
        }
        private Type GetDataType(string name)
        {
            Type? type = Assembly.Load("Entities")
                                 .GetTypes()
                                 .Where(f => f.GetCustomAttributes(typeof(ExcelObject), false).Length > 0)
                                 .Single(f => f.GetCustomAttribute<ExcelObject>()?.Title == name || f.Name == name);

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
