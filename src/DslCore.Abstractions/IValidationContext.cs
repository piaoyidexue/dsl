namespace DslCore.Abstractions
{
    public interface IValidationContext
    {
        IDslLogger Logger { get; }
        bool SymbolExists(string id);
        bool ActionExists(string actionId);
    }
}
