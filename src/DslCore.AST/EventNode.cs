using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class EventNode : DslNode
    {
        public string EventType { get; }
        public ConditionNode? Condition { get; }
        public IReadOnlyList<ActionNode> Actions { get; }
        public EventNode(string id, string eventType, ConditionNode? condition = null, IReadOnlyList<ActionNode>? actions = null, SourceLocation location = default)
            : base(id, location)
        {
            EventType = eventType ?? "";
            Condition = condition;
            Actions = actions ?? new List<ActionNode>();
        }
    }
}
