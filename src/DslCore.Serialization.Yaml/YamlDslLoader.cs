using System;
using System.Collections.Generic;
using System.Linq;
using DslCore.AST;
using DslCore.Serialization.Yaml.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DslCore.Serialization.Yaml
{
    public sealed class YamlDslLoader
    {
        private readonly IDeserializer _deserializer;
        public YamlDslLoader()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
        }
        public RootNode Load(string yamlText)
        {
            if (yamlText is null) throw new ArgumentNullException(nameof(yamlText));
            var yamlRoot = _deserializer.Deserialize<YamlRoot>(yamlText);
            if (yamlRoot == null) throw new InvalidOperationException("Failed to deserialize YAML");
            return MapRoot(yamlRoot);
        }
        public RootNode LoadFromFile(string filePath)
        {
            var text = System.IO.File.ReadAllText(filePath);
            return Load(text);
        }
        private RootNode MapRoot(YamlRoot yaml)
        {
            return new RootNode(
                yaml.Id,
                yaml.Rules?.Select(MapRule).ToList(),
                yaml.Flows?.Select(MapFlow).ToList(),
                yaml.StateMachines?.Select(MapStateMachine).ToList(),
                yaml.Events?.Select(MapEvent).ToList(),
                yaml.AIGraphs?.Select(MapAIGraph).ToList(),
                yaml.SpawnConfigs?.Select(MapSpawnConfig).ToList());
        }
        private RuleNode MapRule(YamlRule yaml)
        {
            return new RuleNode(
                yaml.Id,
                yaml.Conditions?.Select(MapCondition).ToList(),
                yaml.Actions?.Select(MapAction).ToList(),
                yaml.Priority);
        }
        private ConditionNode MapCondition(YamlCondition yaml)
        {
            var kind = yaml.Kind.ToLowerInvariant() switch
            {
                "and" => ConditionKind.And,
                "or" => ConditionKind.Or,
                "not" => ConditionKind.Not,
                _ => ConditionKind.Expression
            };
            if (kind == ConditionKind.Expression)
                return new ConditionNode(yaml.Id, yaml.Expression ?? "");
            return new ConditionNode(yaml.Id, kind, yaml.Children?.Select(MapCondition).ToList() ?? new List<ConditionNode>());
        }
        private ActionNode MapAction(YamlAction yaml)
        {
            return new ActionNode(yaml.Id, yaml.Name, yaml.Parameters);
        }
        private FlowGraph MapFlow(YamlFlow yaml)
        {
            return new FlowGraph(
                yaml.Id,
                yaml.EntryNodeId,
                yaml.Nodes?.Select(MapFlowNode).ToList(),
                yaml.Edges?.Select(MapFlowEdge).ToList());
        }
        private FlowNode MapFlowNode(YamlFlowNode yaml)
        {
            return new FlowNode(
                yaml.Id,
                yaml.OnEnterActions?.Select(MapAction).ToList(),
                yaml.OnExitActions?.Select(MapAction).ToList());
        }
        private FlowEdge MapFlowEdge(YamlFlowEdge yaml)
        {
            var kind = yaml.Kind.ToLowerInvariant() switch
            {
                "conditional" => EdgeKind.Conditional,
                "weighted" => EdgeKind.Weighted,
                "event" => EdgeKind.EventTriggered,
                "event_triggered" => EdgeKind.EventTriggered,
                _ => EdgeKind.Unconditional
            };
            return new FlowEdge(
                yaml.Id,
                yaml.SourceNodeId,
                yaml.TargetNodeId,
                kind,
                yaml.Condition != null ? MapCondition(yaml.Condition) : null,
                yaml.Weight,
                yaml.TriggerEvent);
        }
        private StateMachineNode MapStateMachine(YamlStateMachine yaml)
        {
            return new StateMachineNode(
                yaml.Id,
                yaml.InitialStateId,
                yaml.States?.Select(MapState).ToList(),
                yaml.Transitions?.Select(MapTransition).ToList());
        }
        private StateNode MapState(YamlState yaml)
        {
            return new StateNode(
                yaml.Id,
                yaml.OnEnterActions?.Select(MapAction).ToList(),
                yaml.OnExitActions?.Select(MapAction).ToList());
        }
        private TransitionNode MapTransition(YamlTransition yaml)
        {
            return new TransitionNode(
                yaml.Id,
                yaml.SourceStateId,
                yaml.TargetStateId,
                yaml.Condition != null ? MapCondition(yaml.Condition) : null,
                yaml.TriggerEvent,
                yaml.Actions?.Select(MapAction).ToList());
        }
        private EventNode MapEvent(YamlEvent yaml)
        {
            return new EventNode(
                yaml.Id,
                yaml.EventType,
                yaml.Condition != null ? MapCondition(yaml.Condition) : null,
                yaml.Actions?.Select(MapAction).ToList());
        }
        private AIGraphNode MapAIGraph(YamlAIGraph yaml)
        {
            var kind = yaml.Kind.ToLowerInvariant() switch
            {
                "utility_ai" => AIGraphKind.UtilityAI,
                "goap" => AIGraphKind.GOAP,
                _ => AIGraphKind.BehaviorTree
            };
            return new AIGraphNode(
                yaml.Id,
                kind,
                yaml.RootChildId,
                yaml.Children?.Select(MapAINode).ToList());
        }
        private AINode MapAINode(YamlAINode yaml)
        {
            var kind = yaml.Kind.ToLowerInvariant() switch
            {
                "sequence" => AINodeKind.Sequence,
                "selector" => AINodeKind.Selector,
                "parallel" => AINodeKind.Parallel,
                "decorator" => AINodeKind.Decorator,
                _ => AINodeKind.Leaf
            };
            return new AINode(
                yaml.Id,
                kind,
                yaml.Condition != null ? MapCondition(yaml.Condition) : null,
                yaml.Action != null ? MapAction(yaml.Action) : null,
                yaml.Children?.Select(MapAINode).ToList());
        }
        private SpawnConfigNode MapSpawnConfig(YamlSpawnConfig yaml)
        {
            return new SpawnConfigNode(
                yaml.Id,
                yaml.EntityId,
                yaml.Count,
                yaml.PositionConstraint,
                yaml.Properties);
        }
    }
}
