using EFDM.Abstractions.DataQueries;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EFDM.Core.DataQueries
{
    public class QueryFilter<T> : IQueryFilter<T>
    {
        public bool IsOr { get; }
        public List<Expression<Func<T, bool>>> Expressions { get; } = new List<Expression<Func<T, bool>>>();
        public List<IQueryFilter<T>> Childs { get; } = new List<IQueryFilter<T>>();

        public QueryFilter(bool or = false)
        {
            IsOr = or;
        }

        public IQueryFilter<T> Add(Expression<Func<T, bool>> fn)
        {
            Expressions.Add(fn);
            return this;
        }

        public IQueryFilter<T> Add(IQueryFilter<T> child)
        {
            if (child.Expressions.Count > 0 || child.Childs.Count > 0)
            {
                if (child.IsOr == IsOr)
                {
                    Expressions.AddRange(child.Expressions);
                    Childs.AddRange(child.Childs);
                }
                else
                    Childs.Add(child);
            }
            return this;
        }

        public IQueryFilter<T> And(Action<IQueryFilter<T>> fn) => _add(fn, false);
        public IQueryFilter<T> Or(Action<IQueryFilter<T>> fn) => _add(fn, true);

        IQueryFilter<T> _add(Action<IQueryFilter<T>> fn, bool mode)
        {
            var node = new QueryFilter<T>(mode);
            fn(node);
            return Add(node);
        }
    }
}
