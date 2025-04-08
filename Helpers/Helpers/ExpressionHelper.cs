using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities.Helpers
{
    public static class ExpressionHelper
    {
        public static Expression ResolveCondition(ParameterExpression parameter, string query)
        {
            string[] pieces = Regex.Split(query, @"(==|!=|>=|<=|>|<)");


            string[] properties = pieces[0].Split('.');
            MemberExpression memExpression = null;

            foreach (var propertyItem in properties)
                memExpression = Expression.PropertyOrField(memExpression == null ? parameter : memExpression,
                                                            propertyItem);

            if (pieces.Length == 1)
                return memExpression;

            var condition = CreateCondition(memExpression, pieces[2], pieces[1]);

            return condition;
        }

        public static Expression CreateCondition(Expression property, object value, string opr)
        {
            var propertyType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
            var convertedValue = Convert.ChangeType(value, propertyType);
            var constant = Expression.Constant(convertedValue);

            var result = CreateBinaryExpression(property, constant, opr);

            return result;
        }

        public static Expression CreateBinaryExpression(Expression left, Expression right, string opr)
        {
            var result = opr switch
            {
                "==" => Expression.Equal(left, right),
                "!=" => Expression.NotEqual(left, right),
                ">" => Expression.GreaterThan(left, right),
                ">=" => Expression.GreaterThanOrEqual(left, right),
                "<" => Expression.LessThan(left, right),
                "<=" => Expression.LessThanOrEqual(left, right),

                "&&" => Expression.AndAlso(left, right),
                "&" => Expression.And(left, right),
                "||" => Expression.OrElse(left, right),
                "|" => Expression.Or(left, right),

                _ => throw new NotSupportedException($"Opr ({opr}) is not avaible!")
            };

            return result;
        }


    }
}
