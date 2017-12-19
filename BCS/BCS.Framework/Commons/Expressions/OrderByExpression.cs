using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace BCS.Framework.Commons.Expressions
{
    internal interface IOrderByExpression<T>
    {
        IQueryable<T> ApplyOrderBy(ref IQueryable<T> query);
    }

    internal class OrderByExpression<T, TKey> : IOrderByExpression<T>
    {
        public IQueryable<T> ApplyOrderBy(ref IQueryable<T> query)
        {
            query = query.OrderBy(exp);

            return query;
        }

        private Expression<Func<T, TKey>> exp = null;

        public OrderByExpression(Expression<Func<T, TKey>> myExpression)
        {
            exp = myExpression;
        }
    }
}
