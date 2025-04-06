using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapters.Models
{
    public class ExcelSchema
    {
        public List<ExcelColumn>? Columns { get; set; }
    }

    public class ExcelColumn
    {
        public string Name { get; set; }
        public string Property { get; set; }

    }
}
