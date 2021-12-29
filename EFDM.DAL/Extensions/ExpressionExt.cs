using EFDM.Abstractions.DataQueries;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFDM.Core.Extensions {

    public static class ExpressionExt {

        public static Expression<Func<T, bool>> ToAnd<T>(this IEnumerable<Expression<Func<T, bool>>> exprs) {
            return _iterate(exprs, (fullExpr, expr) => PredicateBuilder.And(fullExpr, expr));
        }
        public static Expression<Func<T, bool>> ToOr<T>(this IEnumerable<Expression<Func<T, bool>>> exprs) {
            return _iterate(exprs, (fullExpr, expr) => PredicateBuilder.Or(fullExpr, expr));
        }

        static Expression<Func<T, bool>> _iterate<T>(IEnumerable<Expression<Func<T, bool>>> exprs, Func<Expression<Func<T, bool>>, Expression<Func<T, bool>>, Expression<Func<T, bool>>> func) {
            Expression<Func<T, bool>> fullExpr = null;
            foreach (var expr in exprs) {
                if (fullExpr == null)
                    fullExpr = expr;
                else
                    fullExpr = func(fullExpr, expr);
            }
            return fullExpr;
        }

        public static Expression<Func<T, bool>> ToExpression<T>(this IQueryFilter<T> filter) {
            var exprs = filter.Expressions.Concat(filter.Childs.Select(x => x.ToExpression()));
            return filter.IsOr ? exprs.ToOr() : exprs.ToAnd();
        }
    }
}
