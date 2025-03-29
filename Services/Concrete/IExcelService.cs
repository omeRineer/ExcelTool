using Microsoft.AspNetCore.Http;
using Models.ExcelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public interface IExcelService
    {
        Stream Export(string key, ExportDataQueryModel query);
        void Import(string key, IFormFile file);
    }
}
