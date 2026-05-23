using System;
using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.Compiler.StateMachine;

namespace DslCore.Runtime
{
    public sealed class StateMachineInstance
    {
        private readonly CompiledStateMachine _stateMachine;
        private string? _currentStateId;
        private bool _isCompleted;
        public string InstanceId { get; }
        public string StateMachineId => _stateMachine.Id;
        public string? CurrentStateId => _currentStateId;
        public bool IsCompleted => _isCompleted;
        public StateMachineInstance(string instanceId, CompiledStateMachine stateMachine)
        {
            InstanceId = instanceId ?? throw new ArgumentNullException(nameof(instanceId));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }
        public void Start(IDslContext context, List<DslCommand> outputCommands)
        {
            var initialState = _stateMachine.InitialStateId;
            if (initialState == null || !_stateMachine.States.ContainsKey(initialState))
            {
                _isCompleted = true;
                return;
            }
            _currentStateId = initialState;
            EnterState(_currentStateId, context, outputCommands);
        }
        public void Tick(IDslContext context, List<DslCommand> outputCommands)
        {
            if (_isCompleted || _currentStateId == null) return;
            foreach (var trans in _stateMachine.Transitions)
            {
                if (trans.SourceStateId != _currentStateId) continue;
                if (trans.TriggerEvent != null) continue;
                if (trans.ConditionEvaluator == null || trans.ConditionEvaluator(context))
                {
                    TransitionTo(trans, context, outputCommands);
                    return;
                }
            }
        }
        public void PostEvent(string eventType, IDslContext context, List<DslCommand> outputCommands)
        {
            if (_isCompleted || _currentStateId == null) return;
            foreach (var trans in _stateMachine.Transitions)
            {
                if (trans.SourceStateId != _currentStateId) continue;
                if (trans.TriggerEvent != eventType) continue;
                if (trans.ConditionEvaluator == null || trans.ConditionEvaluator(context))
                {
                    TransitionTo(trans, context, outputCommands);
                    return;
                }
            }
        }
        private void TransitionTo(CompiledTransition transition, IDslContext context, List<DslCommand> outputCommands)
        {
            if (_currentStateId != null && _stateMachine.States.TryGetValue(_currentStateId, out var currentState))
                outputCommands.AddRange(currentState.OnExitCommands);
            outputCommands.AddRange(transition.TransitionCommands);
            _currentStateId = transition.TargetStateId;
            if (!_stateMachine.States.ContainsKey(transition.TargetStateId))
            {
                _isCompleted = true;
                return;
            }
            EnterState(transition.TargetStateId, context, outputCommands);
        }
        private void EnterState(string stateId, IDslContext context, List<DslCommand> outputCommands)
        {
            if (_stateMachine.States.TryGetValue(stateId, out var state))
                outputCommands.AddRange(state.OnEnterCommands);
        }
    }
}
