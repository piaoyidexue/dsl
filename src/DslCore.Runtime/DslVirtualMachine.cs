using System;
using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.Compiler;
using DslCore.Compiler.Flow;
using DslCore.Compiler.StateMachine;

namespace DslCore.Runtime
{
    public sealed class DslVirtualMachine
    {
        private readonly RuntimeSpec _spec;
        private readonly IDslContext _context;
        private readonly ActionDispatcher _actionDispatcher;
        private readonly Dictionary<string, CompiledFlowGraph> _flowGraphs;
        private readonly Dictionary<string, CompiledStateMachine> _stateMachines;
        private readonly List<FlowInstance> _flowInstances = new List<FlowInstance>();
        private readonly List<StateMachineInstance> _smInstances = new List<StateMachineInstance>();
        private int _instanceCounter;
        public IReadOnlyList<FlowInstance> FlowInstances => _flowInstances;
        public IReadOnlyList<StateMachineInstance> StateMachineInstances => _smInstances;
        public DslVirtualMachine(RuntimeSpec spec, IDslContext context, ActionDispatcher actionDispatcher)
        {
            _spec = spec ?? throw new ArgumentNullException(nameof(spec));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _actionDispatcher = actionDispatcher ?? throw new ArgumentNullException(nameof(actionDispatcher));
            _flowGraphs = spec.FlowGraphs.ToDictionary(f => f.Id);
            _stateMachines = spec.StateMachines.ToDictionary(sm => sm.Id);
        }
        public FlowInstance StartFlow(string flowId)
        {
            if (!_flowGraphs.TryGetValue(flowId, out var graph))
                throw new ArgumentException($"Flow graph '{flowId}' not found", nameof(flowId));
            var instanceId = $"flow_{++_instanceCounter}";
            var instance = new FlowInstance(instanceId, graph);
            var commands = new List<DslCommand>();
            instance.Start(_context, commands);
            _actionDispatcher.DispatchAll(commands, _context);
            _flowInstances.Add(instance);
            return instance;
        }
        public StateMachineInstance StartStateMachine(string stateMachineId)
        {
            if (!_stateMachines.TryGetValue(stateMachineId, out var sm))
                throw new ArgumentException($"State machine '{stateMachineId}' not found", nameof(stateMachineId));
            var instanceId = $"sm_{++_instanceCounter}";
            var instance = new StateMachineInstance(instanceId, sm);
            var commands = new List<DslCommand>();
            instance.Start(_context, commands);
            _actionDispatcher.DispatchAll(commands, _context);
            _smInstances.Add(instance);
            return instance;
        }
        public void Tick()
        {
            var commands = new List<DslCommand>();
            foreach (var flow in _flowInstances)
            {
                if (flow.IsCompleted) continue;
                flow.Tick(_context, commands);
            }
            foreach (var sm in _smInstances)
            {
                if (sm.IsCompleted) continue;
                sm.Tick(_context, commands);
            }
            _actionDispatcher.DispatchAll(commands, _context);
            _flowInstances.RemoveAll(f => f.IsCompleted);
            _smInstances.RemoveAll(s => s.IsCompleted);
        }
        public void PostEvent(DslEvent dslEvent)
        {
            var commands = new List<DslCommand>();
            foreach (var flow in _flowInstances)
            {
                if (flow.IsCompleted) continue;
                flow.PostEvent(dslEvent.EventType, _context, commands);
            }
            foreach (var sm in _smInstances)
            {
                if (sm.IsCompleted) continue;
                sm.PostEvent(dslEvent.EventType, _context, commands);
            }
            _actionDispatcher.DispatchAll(commands, _context);
        }
    }
}
