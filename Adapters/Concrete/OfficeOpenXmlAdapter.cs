using Adapters.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Helpers;

namespace Adapters.Concrete
{
    public class OfficeOpenXmlAdapter : IExcelAdapter
    {
        public IList<Dictionary<string, object>> ConvertToDictionary(Stream stream)
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

        public Stream Create(IList<KeyValuePair<string, string>> columns, IList<object> data, SchemaOptions options = null)
        {
            using (var excelPackage = new ExcelPackage())
            {
                var sheet = excelPackage.Workbook.Worksheets.Add(options.SheetName ?? "Collection");

                for (int i = 1; i <= columns.Count; i++)
                {
                    var cell = sheet.Cells[1, i];

                    cell.Value = columns[i - 1].Key;
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(options.CellColor ?? Color.Transparent);

                    for (int j = 0; j < data.Count; j++)
                        sheet.Cells[j + 2, i].Value = ReflectionHelper.GetPropertyValue(data[j], columns[i - 1].Value);
                }

                return excelPackage.Stream;
            }
        }

        private IList<string> GetColumnNames(ExcelWorksheet sheet)
        {
            List<string> columnNames = new List<string>();

            for (int col = 1; col <= sheet.Dimension.Columns; col++)
                columnNames.Add(sheet.Cells[1, col].Text);

            return columnNames;
        }
    }
}
