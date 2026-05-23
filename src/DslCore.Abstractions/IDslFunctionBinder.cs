namespace DslCore.Abstractions
{
    public interface IDslFunctionBinder
    {
        object? Invoke(string functionName, object?[] args, IDslContext context);
        bool HasFunction(string functionName);
    }
}
