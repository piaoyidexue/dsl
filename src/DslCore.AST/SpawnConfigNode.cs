using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class SpawnConfigNode : DslNode
    {
        public string EntityId { get; }
        public int Count { get; }
        public string? PositionConstraint { get; }
        public IReadOnlyDictionary<string, object?> Properties { get; }
        public SpawnConfigNode(string id, string entityId, int count = 1, string? positionConstraint = null, IReadOnlyDictionary<string, object?>? properties = null, SourceLocation location = default)
            : base(id, location)
        {
            EntityId = entityId ?? "";
            Count = count;
            PositionConstraint = positionConstraint;
            Properties = properties ?? new Dictionary<string, object?>();
        }
    }
}
