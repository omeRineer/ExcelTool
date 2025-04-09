using Utilities.Attributes;

namespace Entities.Concrete
{
    [ExcelObject]
    public class Category : BaseEntity
    {
        [ExcelProperty]
        public string Name { get; set; }
    }
}
