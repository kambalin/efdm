using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFDM.DAL.Helpers;

internal static class SetPropertyCallsParser
{
    internal record SetterInfo(string PropertyName, Func<object, object> GetNewValue);

    internal static IList<SetterInfo> Parse<TEntity>(
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expr)
    {
        var result = new List<SetterInfo>();
        WalkChain(expr.Body, result, typeof(TEntity));
        return result;
    }

    private static void WalkChain(Expression node, List<SetterInfo> result, Type entityType)
    {
        if (node is not MethodCallExpression call || call.Method.Name != "SetProperty")
            return;

        WalkChain(call.Object, result, entityType);

        var propName = ExtractPropertyName(call.Arguments[0]);
        if (propName == null)
            return;

        var valueGetter = CompileValueGetter(call.Arguments[1], entityType);
        if (valueGetter == null)
            return;

        result.Add(new SetterInfo(propName, valueGetter));
    }

    private static string ExtractPropertyName(Expression arg)
    {
        var lambda = Unwrap(arg);
        return (lambda?.Body as MemberExpression)?.Member.Name;
    }

    private static Func<object, object> CompileValueGetter(Expression arg, Type entityType)
    {
        try
        {
            var lambda = Unwrap(arg);
            if (lambda == null)
                return null;

            var param = Expression.Parameter(typeof(object), "entity");
            var cast = Expression.Convert(param, entityType);
            var visitor = new ParameterReplacer(lambda.Parameters[0], cast);
            var newBody = visitor.Visit(lambda.Body);
            var boxed = Expression.Convert(newBody, typeof(object));
            return Expression.Lambda<Func<object, object>>(boxed, param).Compile();
        }
        catch
        {
            return null;
        }
    }

    private static LambdaExpression Unwrap(Expression expr)
    {
        if (expr is UnaryExpression u && u.NodeType == ExpressionType.Quote)
            return u.Operand as LambdaExpression;
        return expr as LambdaExpression;
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _target;
        private readonly Expression _replacement;

        internal ParameterReplacer(ParameterExpression target, Expression replacement)
        {
            _target = target;
            _replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _target ? _replacement : base.VisitParameter(node);
    }
}
