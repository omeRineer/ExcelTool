using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeArchitecture.Reporting.Attributes;

namespace Entities.Concrete
{
    [ReportObject]
    public class Product : BaseEntity
    {
        public Guid CategoryId { get; set; }
        [ReportProperty]
        public string Name { get; set; }
        [ReportProperty]
        public decimal Price { get; set; }
        [ReportProperty]
        public int Quantity { get; set; }
        public Category Category { get; set; }
    }
}
