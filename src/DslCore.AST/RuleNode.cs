using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class RuleNode : DslNode
    {
        public IReadOnlyList<ConditionNode> Conditions { get; }
        public IReadOnlyList<ActionNode> Actions { get; }
        public int Priority { get; }
        public RuleNode(string id, IReadOnlyList<ConditionNode>? conditions, IReadOnlyList<ActionNode>? actions, int priority = 0, SourceLocation location = default)
            : base(id, location)
        {
            Conditions = conditions ?? new List<ConditionNode>();
            Actions = actions ?? new List<ActionNode>();
            Priority = priority;
        }
    }
}
