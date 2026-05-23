using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class RootNode : DslNode
    {
        public IReadOnlyList<RuleNode> Rules { get; }
        public IReadOnlyList<FlowGraph> Flows { get; }
        public IReadOnlyList<StateMachineNode> StateMachines { get; }
        public IReadOnlyList<EventNode> Events { get; }
        public IReadOnlyList<AIGraphNode> AIGraphs { get; }
        public IReadOnlyList<SpawnConfigNode> SpawnConfigs { get; }
        public RootNode(
            string id,
            IReadOnlyList<RuleNode>? rules = null,
            IReadOnlyList<FlowGraph>? flows = null,
            IReadOnlyList<StateMachineNode>? stateMachines = null,
            IReadOnlyList<EventNode>? events = null,
            IReadOnlyList<AIGraphNode>? aiGraphs = null,
            IReadOnlyList<SpawnConfigNode>? spawnConfigs = null,
            SourceLocation location = default) : base(id, location)
        {
            Rules = rules ?? new List<RuleNode>();
            Flows = flows ?? new List<FlowGraph>();
            StateMachines = stateMachines ?? new List<StateMachineNode>();
            Events = events ?? new List<EventNode>();
            AIGraphs = aiGraphs ?? new List<AIGraphNode>();
            SpawnConfigs = spawnConfigs ?? new List<SpawnConfigNode>();
        }
    }
}
