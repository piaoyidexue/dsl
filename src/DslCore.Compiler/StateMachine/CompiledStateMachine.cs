using System.Collections.Generic;

namespace DslCore.Compiler.StateMachine
{
    public sealed class CompiledStateMachine
    {
        public string Id { get; }
        public string? InitialStateId { get; }
        public IReadOnlyDictionary<string, CompiledState> States { get; }
        public IReadOnlyList<CompiledTransition> Transitions { get; }
        public CompiledStateMachine(string id, string? initialStateId, IReadOnlyDictionary<string, CompiledState> states, IReadOnlyList<CompiledTransition> transitions)
        {
            Id = id; InitialStateId = initialStateId; States = states; Transitions = transitions;
        }
    }
}
