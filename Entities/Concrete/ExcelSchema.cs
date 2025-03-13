using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class ExcelSchema : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public string Schema { get; set; }
        public int Type { get; set; }
    }
}
