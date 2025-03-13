using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ExcelSchemas
{
    public class ExcelImportSchema
    {
        public List<ImportSchemaColumn> Columns { get; set; }

    }

    public class ImportSchemaColumn
    {
        public string Name { get; set; }
        public string Property { get; set; }
        public string Type { get; set; }

    }
}
