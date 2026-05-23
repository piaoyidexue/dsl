using System;
using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.Compiler.StateMachine
{
    public sealed class CompiledTransition
    {
        public string Id { get; }
        public string SourceStateId { get; }
        public string TargetStateId { get; }
        public Func<IDslContext, bool>? ConditionEvaluator { get; }
        public string? TriggerEvent { get; }
        public IReadOnlyList<DslCommand> TransitionCommands { get; }
        public CompiledTransition(string id, string sourceStateId, string targetStateId, Func<IDslContext, bool>? conditionEvaluator, string? triggerEvent, IReadOnlyList<DslCommand> transitionCommands)
        {
            Id = id; SourceStateId = sourceStateId; TargetStateId = targetStateId; ConditionEvaluator = conditionEvaluator; TriggerEvent = triggerEvent; TransitionCommands = transitionCommands;
        }
    }
}
