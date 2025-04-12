using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcelObject : Attribute
    {
        public string Title { get; }
        public ExcelObject(string title = null)
        {
            Title = title;
        }
    }
}
