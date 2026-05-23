using System;

using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;
using DslCore.Compiler.Expressions;

namespace DslCore.Runtime
{
    public sealed class ConditionEvaluator(ExpressionCompiler expressionCompiler)
    {
        public bool Evaluate(ConditionNode condition, IDslContext context)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return condition.Kind switch
            {
                ConditionKind.Expression => EvaluateExpression(condition.Expression!, context),
                ConditionKind.And => condition.Children.All(c => Evaluate(c, context)),
                ConditionKind.Or => condition.Children.Any(c => Evaluate(c, context)),
                ConditionKind.Not => condition.Children.Count > 0 && !Evaluate(condition.Children[0], context),
                _ => false
            };
        }
        private bool EvaluateExpression(string expression, IDslContext context)
        {
            var compiled = expressionCompiler.Compile(expression);
            var result = compiled(context);
            return IsTruthy(result);
        }
        private static bool IsTruthy(object? value)
        {
            if (value == null) return false;
            if (value is bool b) return b;
            if (value is int i) return i != 0;
            if (value is double d) return d != 0.0;
            if (value is string s) return !string.IsNullOrEmpty(s);
            return true;
        }
    }
}
