using Core.Utilities.Helpers;
using Models.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities.Extensions;
using Utilities.Helpers;

namespace Utilities.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, QueryParameters queryModel)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "f");
            Expression condition = null;

            foreach (var filterQuery in queryModel.FilterQueries)
            {
                var resolvedCondition = ExpressionHelper.ResolveCondition(parameter, filterQuery.Condition);

                if (condition == null)
                    condition = resolvedCondition;
                else if (condition != null && !string.IsNullOrEmpty(filterQuery.Operator))
                    condition = ExpressionHelper.CreateBinaryExpression(condition, resolvedCondition, filterQuery.Operator);
            }

            if (condition == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, QueryParameters queryModel)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "o");
            IQueryable<T> orderByQuery = null;

            foreach (var sortObject in queryModel.SortQueries)
            {
                bool isFirstIndex = sortObject == queryModel.SortQueries[0];
                var propertyInfo = ReflectionHelper.GetProperty(type, sortObject.Field);
                var memberExpression = ExpressionHelper.ResolveCondition(parameter, sortObject.Field);
                var sortExpression = Expression.Lambda(memberExpression, parameter);

                string orderMethod = GetOrderMethod(sortObject.Direction, !isFirstIndex);
                orderByQuery = ApplyOrdering(isFirstIndex ? query : orderByQuery,
                                             orderMethod,
                                             propertyInfo.Info.PropertyType,
                                             sortExpression);
            }

            return orderByQuery ?? query;
        }

        #region Private

        static IOrderedQueryable<T> ApplyOrdering<T>(IQueryable<T> source,
                                                      string methodName,
                                                      Type propertyType,
                                                      LambdaExpression keySelector)
        {
            var sortExpression = Expression.Call(
                                                    typeof(Queryable),
                                                    methodName,
                                                    new Type[] { typeof(T), propertyType },
                                                    source.Expression,
                                                    Expression.Quote(keySelector)
                                                );

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(sortExpression);
        }

        static string GetOrderMethod(string direction, bool isThen = false)
        {
            return direction.ToLower() switch
            {
                "asc" => !isThen ? "OrderBy" : "ThenBy",
                "desc" => !isThen ? "OrderByDescending" : "ThenByDescending",

                _ => throw new NotSupportedException($"Opr ({direction}) is not supported!")
            };

        }
        #endregion
    }


}
