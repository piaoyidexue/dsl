using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class StateMachineNode : DslNode
    {
        public string? InitialStateId { get; }
        public IReadOnlyList<StateNode> States { get; }
        public IReadOnlyList<TransitionNode> Transitions { get; }
        public StateMachineNode(string id, string? initialStateId, IReadOnlyList<StateNode>? states, IReadOnlyList<TransitionNode>? transitions, SourceLocation location = default)
            : base(id, location)
        {
            InitialStateId = initialStateId;
            States = states ?? new List<StateNode>();
            Transitions = transitions ?? new List<TransitionNode>();
        }
    }
}
