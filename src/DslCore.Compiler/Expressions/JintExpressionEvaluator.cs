using System;
using System.Globalization;
using DslCore.Abstractions;
using Jint;
using Jint.Runtime;

namespace DslCore.Compiler.Expressions
{
    public sealed class JintExpressionEvaluator : IExpressionEvaluator
    {
        private readonly Engine _engine;
        private bool _disposed;
        public JintExpressionEvaluator()
        {
            _engine = new Engine(options =>
            {
                options.Strict();
                options.LimitRecursion(100);
                options.TimeoutInterval(TimeSpan.FromSeconds(5));
                options.MaxStatements(10000);
            });
        }
        public object? Evaluate(string expression, IDslContext context)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(JintExpressionEvaluator));
            SyncContextToEngine(context);
            try
            {
                var result = _engine.Evaluate(expression);
                return result.ToObject();
            }
            catch (JavaScriptException ex)
            {
                throw new DslExpressionException($"Expression evaluation failed: {ex.Message}", ex);
            }
        }
        public T? Evaluate<T>(string expression, IDslContext context)
        {
            var result = Evaluate(expression, context);
            if (result == null) return default;
            return (T)Convert.ChangeType(result, typeof(T), CultureInfo.InvariantCulture);
        }
        public void SetVariable(string name, object? value)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(JintExpressionEvaluator));
            _engine.SetValue(name, value ?? "null");
        }
        public void RegisterFunction(string name, Func<object?[], object?> func)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(JintExpressionEvaluator));
            _engine.SetValue(name, func);
        }
        private  static void SyncContextToEngine(IDslContext context)
        {
        }
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
