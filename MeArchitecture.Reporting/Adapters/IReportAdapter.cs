using MeArchitecture.Reporting.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeArchitecture.Reporting.Adapters
{
    public interface IReportAdapter
    {
        Task<Stream> CreateAsync<TData>(IList<KeyValuePair<string, string>> columns,
                                 IList<TData> data,
                                 SchemaOptions options = null) where TData : class, new();
        Task<IList<Dictionary<string, object>>> ReadToDictionaryAsync(Stream stream);
    }
}
