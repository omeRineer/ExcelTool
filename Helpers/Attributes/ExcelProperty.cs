using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelProperty : Attribute
    {
        public ExcelProperty()
        {

        }
    }
}
