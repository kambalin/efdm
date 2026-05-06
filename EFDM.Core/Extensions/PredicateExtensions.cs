using System;
using System.Linq.Expressions;

namespace EFDM.Core.Extensions;

public static class PredicateExtensions
{
    /// <summary>
    /// Позволяет объединять два выражения в одно с помощью логического OR.
    /// Это даёт возможность формировать сложные EF-совместимые условия вида(A AND B) OR(C AND D)
    /// и корректно передавать их в QueryFilter как единое дерево выражения, которое может быть переведено в SQL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr1"></param>
    /// <param name="expr2"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invoked = Expression.Invoke(expr2, expr1.Parameters);

        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(expr1.Body, invoked),
            expr1.Parameters
        );
    }
}
