using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Query
{
    public class QueryParameters
    {
        public List<FilterQuery>? FilterQueries { get; set; }
        public List<SortQuery>? SortQueries { get; set; }
    }

    public class FilterQuery
    {
        public string Condition { get; init; }
        public string? Operator { get; init; }
    }

    public class SortQuery
    {
        public string Field { get; init; }
        public string Direction { get; init; }
    }
}
