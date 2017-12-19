using System;
using System.Linq.Expressions;

namespace BCS.Framework.Commons.Expressions
{
    internal static class ExpressionHelper
    {
        public static Expression<Func<T, TField>> GetExpressionMember<T, TField>(string memberName)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "r");

            MemberExpression memberExpression = Expression.PropertyOrField(parameterExpression, memberName);

            Expression<Func<T, TField>> member = Expression.Lambda<Func<T, TField>>(memberExpression, parameterExpression);

            return member;
        }
    }
}