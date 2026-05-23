using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class TransitionNode : DslNode
    {
        public string SourceStateId { get; }
        public string TargetStateId { get; }
        public ConditionNode? Condition { get; }
        public string? TriggerEvent { get; }
        public IReadOnlyList<ActionNode> Actions { get; }
        public TransitionNode(string id, string sourceStateId, string targetStateId, ConditionNode? condition = null, string? triggerEvent = null, IReadOnlyList<ActionNode>? actions = null, SourceLocation location = default)
            : base(id, location)
        {
            SourceStateId = sourceStateId ?? "";
            TargetStateId = targetStateId ?? "";
            Condition = condition;
            TriggerEvent = triggerEvent;
            Actions = actions ?? new List<ActionNode>();
        }
    }
}
