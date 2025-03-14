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

namespace Services.Concrete
{
    public class CoreExcelService : IExcelService
    {
        readonly CoreContext Context;

        public CoreExcelService(CoreContext context)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Context = context;
        }

        public void Import(string entityType, IFormFile file)
        {
            var schemaData = Context.Set<ExcelSchema>().FirstOrDefault(f => f.Key == entityType);
            var excelSchema = JsonConvert.DeserializeObject<ExcelImportSchema>(schemaData.Schema);

            var dataCollection = ConvertToDictionary(file.OpenReadStream());
            var dataType = GetDataType(entityType);

            foreach (var row in dataCollection)
            {
                var instance = Activator.CreateInstance(dataType);

                foreach (var column in excelSchema.Columns)
                {
                    var value = ChangeType(row[column.Name], column.Type);

                    ReflectionHelper.SetPropertyValue(instance, column.Property, value);
                }

                Context.Add(instance);
            }

            Context.SaveChanges();
        }

        public Stream Export(string entityType, ExportDataQueryModel query)
        {
            var excelSchema = Context.Set<ExcelSchema>().FirstOrDefault(f => f.Key == entityType);
            var schema = JsonConvert.DeserializeObject<ExcelExportSchema>(excelSchema.Schema);
            Type type = GetDataType(entityType);
            var data = GetDataWithDynamic(type, schema, query);

            using (var excelPackage = new ExcelPackage())
            {
                var sheet = excelPackage.Workbook.Worksheets.Add($"{entityType} List");

                for (int i = 1; i <= schema?.Columns?.Count; i++)
                {
                    var cell = sheet.Cells[1, i];

                    cell.Value = schema.Columns[i - 1].Name;
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    if (schema?.CellColor != null)
                        cell.Style.Fill.BackgroundColor.SetColor(Color.FromName(schema.CellColor));

                    for (int j = 0; j < data.Count; j++)
                        sheet.Cells[j + 2, i].Value = ReflectionHelper.GetPropertyValue(data[j], schema?.Columns[i - 1]?.Property);
                }

                var memoryStream = new MemoryStream();
                excelPackage.SaveAs(memoryStream);
                memoryStream.Position = 0;

                return memoryStream;
            }

        }

        #region Private
        private List<string> GetColumnNames(ExcelWorksheet sheet)
        {
            List<string> columnNames = new List<string>();

            for (int col = 1; col <= sheet.Dimension.Columns; col++)
                columnNames.Add(sheet.Cells[1, col].Text);

            return columnNames;
        }
        private List<Dictionary<string, object>> ConvertToDictionary(Stream stream)
        {
            var dataCollection = new List<Dictionary<string, object>>();

            using (var excelPackage = new ExcelPackage(stream))
            {
                var sheets = excelPackage.Workbook.Worksheets;

                foreach (var sheet in sheets)
                {
                    var columnNames = GetColumnNames(sheet);

                    for (var row = 2; row <= sheet.Dimension.Rows; row++)
                    {
                        var rowItem = new Dictionary<string, object>();

                        for (int col = 1; col <= sheet.Dimension.Columns; col++)
                            rowItem.Add(columnNames[col - 1], sheet.Cells[row, col].Value);

                        dataCollection.Add(rowItem);
                    }
                }
            }

            return dataCollection;
        }
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
        private List<object> GetDataWithDynamic(Type type, ExcelExportSchema excelExportShema, ExportDataQueryModel query)
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

        #endregion
    }
}
