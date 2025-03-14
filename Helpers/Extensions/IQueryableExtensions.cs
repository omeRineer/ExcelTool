using Models.ExpressionModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Helpers;

namespace Utilities.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, QueryParameters queryModel)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "f");
            Expression filterQuery = null;

            foreach (var filterObject in queryModel.FilterObjects)
            {
                var memberExpression = GetNestedPropertyExpression(parameter, filterObject.Field);
                var condition = CreateCondition(memberExpression, filterObject.Value, filterObject.Operator);

                filterQuery = filterQuery == null ? condition
                                                  : Expression.AndAlso(filterQuery, condition);
            }

            if (filterQuery == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(filterQuery, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, QueryParameters queryModel)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "o");
            IQueryable<T> orderByQuery = null;

            foreach (var sortObject in queryModel.SortObjects)
            {
                var propertyInfo = ReflectionHelper.GetProperty(type, sortObject.Field);
                var memberExpression = GetNestedPropertyExpression(parameter, sortObject.Field);
                var sortExpression = Expression.Lambda(memberExpression, parameter);

                bool isFirstIndex = sortObject == queryModel.SortObjects[0];
                string orderMethod = GetOrderMethod(sortObject.Direction, !isFirstIndex);
                orderByQuery = ApplyOrdering(isFirstIndex ? query : orderByQuery, 
                                             orderMethod, 
                                             propertyInfo.Info.PropertyType, 
                                             sortExpression);
            }

            return orderByQuery ?? query;
        }

        #region Private
        static MemberExpression GetNestedPropertyExpression(ParameterExpression parameter, string propertyPath)
        {
            string[] properties = propertyPath.Split('.');
            Expression expression = parameter;

            foreach (var property in properties)
                expression = Expression.PropertyOrField(expression, property);

            return (MemberExpression)expression;
        }
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
            if(!isThen)
                return direction.Equals("asc", StringComparison.OrdinalIgnoreCase)
                                            ? "OrderBy"
                                            : "OrderByDescending";
            else
                return direction.Equals("asc", StringComparison.OrdinalIgnoreCase)
                                            ? "ThenBy"
                                            : "ThenByDescending";

        }
        #endregion
    }


}
