using System;
using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;
using DslCore.Compiler.Flow;

namespace DslCore.Runtime
{
    public sealed class FlowInstance
    {
        private readonly CompiledFlowGraph _graph;
        private string? _currentNodeId;
        private readonly HashSet<string> _waitingEvents = new HashSet<string>();
        private bool _isCompleted;
        public string InstanceId { get; }
        public string FlowId => _graph.Id;
        public string? CurrentNodeId => _currentNodeId;
        public bool IsCompleted => _isCompleted;
        public FlowInstance(string instanceId, CompiledFlowGraph graph)
        {
            InstanceId = instanceId ?? throw new ArgumentNullException(nameof(instanceId));
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
        public void Start(IDslContext context, List<DslCommand> outputCommands)
        {
            var entryId = _graph.EntryNodeId;
            if (entryId == null || !_graph.Nodes.ContainsKey(entryId))
            {
                _isCompleted = true;
                return;
            }
            _currentNodeId = entryId;
            EnterNode(_currentNodeId, context, outputCommands);
            CollectWaitingEvents();
        }
        public void Tick(IDslContext context, List<DslCommand> outputCommands)
        {
            if (_isCompleted || _currentNodeId == null) return;
            foreach (var edge in _graph.Edges)
            {
                if (edge.SourceNodeId != _currentNodeId) continue;
                if (edge.Kind == EdgeKind.EventTriggered) continue;
                bool canTraverse = false;
                switch (edge.Kind)
                {
                    case EdgeKind.Unconditional:
                        canTraverse = true;
                        break;
                    case EdgeKind.Conditional:
                        canTraverse = edge.ConditionEvaluator?.Invoke(context) ?? false;
                        break;
                    case EdgeKind.Weighted:
                        canTraverse = edge.ConditionEvaluator?.Invoke(context) ?? true;
                        break;
                }
                if (canTraverse)
                {
                    TraverseTo(edge.TargetNodeId, context, outputCommands);
                    return;
                }
            }
        }
        public void PostEvent(string eventType, IDslContext context, List<DslCommand> outputCommands)
        {
            if (_isCompleted || _currentNodeId == null) return;
            if (!_waitingEvents.Contains(eventType)) return;
            foreach (var edge in _graph.Edges)
            {
                if (edge.SourceNodeId != _currentNodeId) continue;
                if (edge.Kind != EdgeKind.EventTriggered) continue;
                if (edge.TriggerEvent != eventType) continue;
                TraverseTo(edge.TargetNodeId, context, outputCommands);
                return;
            }
        }
        private void TraverseTo(string targetNodeId, IDslContext context, List<DslCommand> outputCommands)
        {
            if (_currentNodeId != null && _graph.Nodes.TryGetValue(_currentNodeId, out var currentNode))
                outputCommands.AddRange(currentNode.OnExitCommands);
            _currentNodeId = targetNodeId;
            if (!_graph.Nodes.ContainsKey(targetNodeId))
            {
                _isCompleted = true;
                return;
            }
            EnterNode(targetNodeId, context, outputCommands);
            CollectWaitingEvents();
            var hasOutgoing = _graph.Edges.Any(e => e.SourceNodeId == targetNodeId);
            if (!hasOutgoing) _isCompleted = true;
        }
        private void EnterNode(string nodeId, IDslContext context, List<DslCommand> outputCommands)
        {
            if (_graph.Nodes.TryGetValue(nodeId, out var node))
                outputCommands.AddRange(node.OnEnterCommands);
        }
        private void CollectWaitingEvents()
        {
            _waitingEvents.Clear();
            if (_currentNodeId == null) return;
            foreach (var edge in _graph.Edges)
            {
                if (edge.SourceNodeId == _currentNodeId && edge.Kind == EdgeKind.EventTriggered && edge.TriggerEvent != null)
                    _waitingEvents.Add(edge.TriggerEvent);
            }
        }
    }
}
