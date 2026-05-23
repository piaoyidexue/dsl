using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace DslCore.Serialization.Yaml.Models
{
    public sealed class YamlRoot
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "rules")]
        public List<YamlRule>? Rules { get; set; }
        [YamlMember(Alias = "flows")]
        public List<YamlFlow>? Flows { get; set; }
        [YamlMember(Alias = "state_machines")]
        public List<YamlStateMachine>? StateMachines { get; set; }
        [YamlMember(Alias = "events")]
        public List<YamlEvent>? Events { get; set; }
        [YamlMember(Alias = "ai_graphs")]
        public List<YamlAIGraph>? AIGraphs { get; set; }
        [YamlMember(Alias = "spawn_configs")]
        public List<YamlSpawnConfig>? SpawnConfigs { get; set; }
    }

    public sealed class YamlRule
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "priority")]
        public int Priority { get; set; }
        [YamlMember(Alias = "conditions")]
        public List<YamlCondition>? Conditions { get; set; }
        [YamlMember(Alias = "actions")]
        public List<YamlAction>? Actions { get; set; }
    }

    public sealed class YamlCondition
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "kind")]
        public string Kind { get; set; } = "expression";
        [YamlMember(Alias = "expression")]
        public string? Expression { get; set; }
        [YamlMember(Alias = "children")]
        public List<YamlCondition>? Children { get; set; }
    }

    public sealed class YamlAction
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "name")]
        public string Name { get; set; } = "";
        [YamlMember(Alias = "params")]
        public Dictionary<string, object?>? Parameters { get; set; }
    }

    public sealed class YamlFlow
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "entry")]
        public string? EntryNodeId { get; set; }
        [YamlMember(Alias = "nodes")]
        public List<YamlFlowNode>? Nodes { get; set; }
        [YamlMember(Alias = "edges")]
        public List<YamlFlowEdge>? Edges { get; set; }
    }

    public sealed class YamlFlowNode
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "on_enter")]
        public List<YamlAction>? OnEnterActions { get; set; }
        [YamlMember(Alias = "on_exit")]
        public List<YamlAction>? OnExitActions { get; set; }
    }

    public sealed class YamlFlowEdge
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "from")]
        public string SourceNodeId { get; set; } = "";
        [YamlMember(Alias = "to")]
        public string TargetNodeId { get; set; } = "";
        [YamlMember(Alias = "kind")]
        public string Kind { get; set; } = "unconditional";
        [YamlMember(Alias = "condition")]
        public YamlCondition? Condition { get; set; }
        [YamlMember(Alias = "weight")]
        public double Weight { get; set; } = 1.0;
        [YamlMember(Alias = "trigger_event")]
        public string? TriggerEvent { get; set; }
    }

    public sealed class YamlStateMachine
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "initial")]
        public string? InitialStateId { get; set; }
        [YamlMember(Alias = "states")]
        public List<YamlState>? States { get; set; }
        [YamlMember(Alias = "transitions")]
        public List<YamlTransition>? Transitions { get; set; }
    }

    public sealed class YamlState
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "on_enter")]
        public List<YamlAction>? OnEnterActions { get; set; }
        [YamlMember(Alias = "on_exit")]
        public List<YamlAction>? OnExitActions { get; set; }
    }

    public sealed class YamlTransition
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "from")]
        public string SourceStateId { get; set; } = "";
        [YamlMember(Alias = "to")]
        public string TargetStateId { get; set; } = "";
        [YamlMember(Alias = "condition")]
        public YamlCondition? Condition { get; set; }
        [YamlMember(Alias = "trigger_event")]
        public string? TriggerEvent { get; set; }
        [YamlMember(Alias = "actions")]
        public List<YamlAction>? Actions { get; set; }
    }

    public sealed class YamlEvent
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "type")]
        public string EventType { get; set; } = "";
        [YamlMember(Alias = "condition")]
        public YamlCondition? Condition { get; set; }
        [YamlMember(Alias = "actions")]
        public List<YamlAction>? Actions { get; set; }
    }

    public sealed class YamlAIGraph
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "kind")]
        public string Kind { get; set; } = "behavior_tree";
        [YamlMember(Alias = "root")]
        public string? RootChildId { get; set; }
        [YamlMember(Alias = "children")]
        public List<YamlAINode>? Children { get; set; }
    }

    public sealed class YamlAINode
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "kind")]
        public string Kind { get; set; } = "leaf";
        [YamlMember(Alias = "condition")]
        public YamlCondition? Condition { get; set; }
        [YamlMember(Alias = "action")]
        public YamlAction? Action { get; set; }
        [YamlMember(Alias = "children")]
        public List<YamlAINode>? Children { get; set; }
    }

    public sealed class YamlSpawnConfig
    {
        [YamlMember(Alias = "id")]
        public string Id { get; set; } = "";
        [YamlMember(Alias = "entity_id")]
        public string EntityId { get; set; } = "";
        [YamlMember(Alias = "count")]
        public int Count { get; set; } = 1;
        [YamlMember(Alias = "position")]
        public string? PositionConstraint { get; set; }
        [YamlMember(Alias = "properties")]
        public Dictionary<string, object?>? Properties { get; set; }
    }
}
