namespace DslCore.Abstractions
{
    public interface IDslContext
    {
        object? GetVariable(string name);
        void SetVariable(string name, object? value);
        T GetService<T>() where T : class;
        bool HasVariable(string name);
    }
}
