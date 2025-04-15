using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeArchitecture.Reporting.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReportObject : Attribute
    {
        public string Title { get; }
        public ReportObject(string title = null)
        {
            Title = title;
        }
    }
}
