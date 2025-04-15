using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeArchitecture.Reporting.Data
{
    public class ReportSchema
    {
        public Guid Id { get; set; } 
        public string Object { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
        public string Schema { get; set; }
        public int Type { get; set; }
    }
}
