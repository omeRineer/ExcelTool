using MeArchitecture.Reporting.Attributes;

namespace Entities.Concrete
{
    [ReportObject]
    public class Category : BaseEntity
    {
        [ReportProperty]
        public string Name { get; set; }
    }
}
