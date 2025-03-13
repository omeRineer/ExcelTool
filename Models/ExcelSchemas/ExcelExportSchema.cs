using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ExcelSchemas
{
    public class ExcelExportSchema
    {
        public string? CellColor { get; set; }
        public string[]? Includes { get; set; }
        public List<ExportSchemaColumn>? Columns { get; set; }

    }

    public class ExportSchemaColumn
    {
        public string Name { get; set; }
        public string Property { get; set; }

    }
}
