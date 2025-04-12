using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ExcelModels
{
    public class ExcelPropertyListModel
    {
        public List<ExcelPropertyList_Item>? Properties { get; set; }
    }

    public class ExcelPropertyList_Item
    {
        public string Title { get; set; }
        public string Type { get; set; }
    }
}
