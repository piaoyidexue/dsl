using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public enum AINodeKind
    {
        Sequence,
        Selector,
        Parallel,
        Decorator,
        Leaf
    }

    public sealed class AINode : DslNode
    {
        public AINodeKind Kind { get; }
        public ConditionNode? Condition { get; }
        public ActionNode? Action { get; }
        public IReadOnlyList<AINode> Children { get; }
        public AINode(string id, AINodeKind kind, ConditionNode? condition = null, ActionNode? action = null, IReadOnlyList<AINode>? children = null, SourceLocation location = default)
            : base(id, location)
        {
            Kind = kind;
            Condition = condition;
            Action = action;
            Children = children ?? new List<AINode>();
        }
    }
}
