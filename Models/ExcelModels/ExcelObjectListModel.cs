using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ExcelModels
{
    public class ExcelObjectListModel
    {
        public List<ExcelObjectList_Item>? Objects { get; set; }
    }

    public class ExcelObjectList_Item
    {
        public string Title { get; set; }
        public string[]? Schemas { get; set; }
    }
}
