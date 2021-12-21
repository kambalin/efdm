using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Abstractions.DataQueries {

    public interface IQueryFilter<T> {
        bool IsOr { get; }
        List<Expression<Func<T, bool>>> Expressions { get; }
        List<IQueryFilter<T>> Childs { get; }

        IQueryFilter<T> Add(Expression<Func<T, bool>> fn);
        IQueryFilter<T> Add(IQueryFilter<T> child);
        IQueryFilter<T> And(Action<IQueryFilter<T>> fn);
        IQueryFilter<T> Or(Action<IQueryFilter<T>> fn);
    }
}
