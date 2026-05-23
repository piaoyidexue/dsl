using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class StateNode : DslNode
    {
        public IReadOnlyList<ActionNode> OnEnterActions { get; }
        public IReadOnlyList<ActionNode> OnExitActions { get; }
        public StateNode(string id, IReadOnlyList<ActionNode>? onEnterActions = null, IReadOnlyList<ActionNode>? onExitActions = null, SourceLocation location = default)
            : base(id, location)
        {
            OnEnterActions = onEnterActions ?? new List<ActionNode>();
            OnExitActions = onExitActions ?? new List<ActionNode>();
        }
    }
}
