using Adapters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapters
{
    public interface IExcelAdapter
    {
        Task<Stream> CreateAsync(IList<KeyValuePair<string, string>> columns, 
                      IList<object> data, 
                      SchemaOptions options = null);
        IList<Dictionary<string, object>> ConvertToDictionary(Stream stream);
    }
}
