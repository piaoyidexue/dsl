using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class ActionNode : DslNode
    {
        public string ActionName { get; }
        public IReadOnlyDictionary<string, object?> Parameters { get; }
        public ActionNode(string id, string actionName, IReadOnlyDictionary<string, object?>? parameters = null, SourceLocation location = default)
            : base(id, location)
        {
            ActionName = actionName ?? "";
            Parameters = parameters ?? new Dictionary<string, object?>();
        }
    }
}
