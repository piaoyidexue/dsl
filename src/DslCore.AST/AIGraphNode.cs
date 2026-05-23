using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public enum AIGraphKind
    {
        BehaviorTree,
        UtilityAI,
        GOAP
    }

    public sealed class AIGraphNode : DslNode
    {
        public AIGraphKind Kind { get; }
        public string? RootChildId { get; }
        public IReadOnlyList<AINode> Children { get; }
        public AIGraphNode(string id, AIGraphKind kind, string? rootChildId = null, IReadOnlyList<AINode>? children = null, SourceLocation location = default)
            : base(id, location)
        {
            Kind = kind;
            RootChildId = rootChildId;
            Children = children ?? new List<AINode>();
        }
    }
}
