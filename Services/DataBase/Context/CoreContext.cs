using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Models.ExcelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Services.DataBase.Context
{
    public class CoreContext : DbContext
    {
        public CoreContext(DbContextOptions opt) : base(opt)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExcelSchema> ExcelSchemas { get; set; }

        public IQueryable<T> GetData<T>(ExportDataQueryModel queryModel, 
                                        string[] includes) 
            where T : class
        {
            IQueryable<T> query = Set<T>();

            if(queryModel.Filter?.FilterObjects != null) query = query.Where(queryModel.Filter);
            if(queryModel.Filter?.SortObjects != null) query = query.OrderBy(queryModel.Filter);

            foreach (var include in includes)
                query = query.Include(include);

            return query.AsQueryable();
        }
    }
}
