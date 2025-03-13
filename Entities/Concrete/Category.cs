using Utilities.Attributes;

namespace Entities.Concrete
{
    [ExcelObject("Category")]
    public class Category : BaseEntity
    {
        public string Name { get; set; }
    }
}
