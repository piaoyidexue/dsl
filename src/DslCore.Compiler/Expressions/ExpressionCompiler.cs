using System;
using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.Compiler.Expressions
{
    public sealed class ExpressionCompiler
    {
        private readonly IExpressionEvaluator _evaluator;
        private readonly Dictionary<string, Func<IDslContext, object?>> _compiled = new Dictionary<string, Func<IDslContext, object?>>();
        public ExpressionCompiler(IExpressionEvaluator evaluator) => _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        public Func<IDslContext, object?> Compile(string expression)
        {
            if (_compiled.TryGetValue(expression, out var cached)) return cached;
            ValidateExpression(expression);
            Func<IDslContext, object?> func = ctx =>
            {
                try { return _evaluator.Evaluate(expression, ctx); }
                catch (DslExpressionException) { return null; }
            };
            _compiled[expression] = func;
            return func;
        }
        public bool TryCompile(string expression, out Func<IDslContext, object?>? compiled, out string? error)
        {
            error = null;
            try
            {
                ValidateExpression(expression);
                compiled = Compile(expression);
                return true;
            }
            catch (DslExpressionException ex)
            {
                error = ex.Message;
                compiled = null;
                return false;
            }
        }
        private void ValidateExpression(string expression)
        {
            try { _evaluator.Evaluate(expression, new EmptyDslContext()); }
            catch (DslExpressionException) { throw; }
            catch (Exception ex) { throw new DslExpressionException(ex.Message, ex); }
        }
        private sealed class EmptyDslContext : IDslContext
        {
            public object? GetVariable(string name) => null;
            public void SetVariable(string name, object? value) { }
            public T GetService<T>() where T : class => throw new InvalidOperationException();
            public bool HasVariable(string name) => false;
        }
    }
}
