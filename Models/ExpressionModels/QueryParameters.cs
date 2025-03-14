using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ExpressionModels
{
    public class QueryParameters
    {
        public List<FilterObject>? FilterObjects { get; set; }
        public List<SortObject>? SortObjects { get; set; }
    }

    public class FilterObject
    {
        public string Field { get; init; }
        public string Operator { get; init; }
        public string Value { get; init; }
    }

    public class SortObject
    {
        public string Field { get; init; }
        public string Direction { get; init; }
    }
}
