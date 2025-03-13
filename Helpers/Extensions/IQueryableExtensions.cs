using Models.ExpressionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, QueryParameters filterModel)
        {
            var parameter = Expression.Parameter(typeof(T), "f");
            Expression filterQuery = null;

            foreach (var filterObject in filterModel.FilterObjects)
            {
                var property = Expression.Property(parameter, filterObject.Field);
                var condition = CreateCondition(property, filterObject.Value, filterObject.Operator);

                filterQuery = filterQuery == null ? condition
                                                  : Expression.AndAlso(filterQuery, condition);
            }

            if (filterQuery == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(filterQuery, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, QueryParameters filterModel)
        {
            var parameter = Expression.Parameter(typeof(T), "o");
            Expression orderQuery = null;


        }

        #region Private
        static Expression CreateCondition(Expression property, object value, string opr)
        {
            var propertyType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
            var convertedValue = Convert.ChangeType(value, propertyType);
            var constant = Expression.Constant(convertedValue);

            var result = opr switch
            {
                "==" => Expression.Equal(property, constant),
                "!=" => Expression.NotEqual(property, constant),
                ">" => Expression.GreaterThan(property, constant),
                ">=" => Expression.GreaterThanOrEqual(property, constant),
                "<" => Expression.LessThan(property, constant),
                "<=" => Expression.LessThanOrEqual(property, constant),

                _ => throw new NotSupportedException($"Opr ({opr}) is not avaible")
            };

            return result;
        }
        #endregion
    }


}
