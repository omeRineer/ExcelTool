using Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    [ExcelObject]
    public class Product : BaseEntity
    {
        public Guid CategoryId { get; set; }
        [ExcelProperty]
        public string Name { get; set; }
        [ExcelProperty]
        public decimal Price { get; set; }
        [ExcelProperty]
        public int Quantity { get; set; }

        [ExcelProperty]
        public Category Category { get; set; }
    }
}
