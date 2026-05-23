using System.Collections.Generic;

namespace DslCore.Abstractions
{
    public interface IDslActionHandler
    {
        void Execute(string actionId, IReadOnlyDictionary<string, object?> parameters, IDslContext context);
        bool CanHandle(string actionId);
    }
}
