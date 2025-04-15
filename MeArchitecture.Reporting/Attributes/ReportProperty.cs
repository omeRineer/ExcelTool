using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeArchitecture.Reporting.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ReportProperty : Attribute
    {
        public ReportProperty()
        {

        }
    }
}
