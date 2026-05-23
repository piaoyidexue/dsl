using System.Collections.Generic;
using System.Linq;

namespace DslCore.Abstractions
{
    public sealed class DslCommand
    {
        public string CommandId { get; }
        public string TargetId { get; }
        public IReadOnlyDictionary<string, object?> Parameters { get; }
        public SourceLocation SourceLocation { get; }
        public DslCommand(string commandId, string targetId, IReadOnlyDictionary<string, object?> parameters, SourceLocation sourceLocation = default)
        {
            CommandId = commandId ?? ""; TargetId = targetId ?? ""; Parameters = parameters ?? new Dictionary<string, object?>(); SourceLocation = sourceLocation;
        }
        public override string ToString() => $"[{CommandId}] {TargetId} {string.Join(",", Parameters.Select(kv => $"{kv.Key}={kv.Value}"))}";
    }
}
