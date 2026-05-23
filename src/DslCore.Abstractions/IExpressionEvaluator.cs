using System;

namespace DslCore.Abstractions
{
    public interface IExpressionEvaluator : IDisposable
    {
        object? Evaluate(string expression, IDslContext context);
        T? Evaluate<T>(string expression, IDslContext context);
        void SetVariable(string name, object? value);
        void RegisterFunction(string name, Func<object?[], object?> func);
    }
}
