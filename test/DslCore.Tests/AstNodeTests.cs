using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;
using FluentAssertions;
using Xunit;

namespace DslCore.Tests
{
    public sealed class AstNodeTests
    {
        [Fact]
        public void RootNode_ShouldInitializeWithDefaults()
        {
            var root = new RootNode("root1");
            root.Id.Should().Be("root1");
            root.Rules.Should().BeEmpty();
            root.Flows.Should().BeEmpty();
            root.StateMachines.Should().BeEmpty();
            root.Events.Should().BeEmpty();
            root.AIGraphs.Should().BeEmpty();
            root.SpawnConfigs.Should().BeEmpty();
        }

        [Fact]
        public void RuleNode_ShouldStoreConditionsAndActions()
        {
            var conditions = new List<ConditionNode> { new ConditionNode("c1", "x > 0") };
            var actions = new List<ActionNode> { new ActionNode("a1", "spawn") };
            var rule = new RuleNode("rule1", conditions, actions, priority: 5);
            rule.Conditions.Should().HaveCount(1);
            rule.Actions.Should().HaveCount(1);
            rule.Priority.Should().Be(5);
        }

        [Fact]
        public void ConditionNode_ExpressionKind_ShouldWork()
        {
            var cond = new ConditionNode("c1", "x > 10");
            cond.Kind.Should().Be(ConditionKind.Expression);
            cond.Expression.Should().Be("x > 10");
            cond.Children.Should().BeEmpty();
        }

        [Fact]
        public void ConditionNode_CompositeKind_ShouldWork()
        {
            var child1 = new ConditionNode("c1", "x > 0");
            var child2 = new ConditionNode("c2", "y < 10");
            var and = new ConditionNode("and1", ConditionKind.And, new List<ConditionNode> { child1, child2 });
            and.Kind.Should().Be(ConditionKind.And);
            and.Children.Should().HaveCount(2);
        }

        [Fact]
        public void FlowGraph_ShouldStoreNodesAndEdges()
        {
            var node1 = new FlowNode("n1");
            var node2 = new FlowNode("n2");
            var edge = new FlowEdge("e1", "n1", "n2");
            var flow = new FlowGraph("flow1", "n1",
                new List<FlowNode> { node1, node2 },
                new List<FlowEdge> { edge });
            flow.EntryNodeId.Should().Be("n1");
            flow.Nodes.Should().HaveCount(2);
            flow.Edges.Should().HaveCount(1);
        }

        [Fact]
        public void StateMachineNode_ShouldStoreStatesAndTransitions()
        {
            var state1 = new StateNode("idle");
            var state2 = new StateNode("combat");
            var trans = new TransitionNode("t1", "idle", "combat");
            var sm = new StateMachineNode("sm1", "idle",
                new List<StateNode> { state1, state2 },
                new List<TransitionNode> { trans });
            sm.InitialStateId.Should().Be("idle");
            sm.States.Should().HaveCount(2);
            sm.Transitions.Should().HaveCount(1);
        }

        [Fact]
        public void SpawnConfigNode_ShouldStoreProperties()
        {
            var props = new Dictionary<string, object?> { { "level", 5 } };
            var spawn = new SpawnConfigNode("sp1", "goblin", count: 3, positionConstraint: "random", properties: props);
            spawn.EntityId.Should().Be("goblin");
            spawn.Count.Should().Be(3);
            spawn.PositionConstraint.Should().Be("random");
            spawn.Properties.Should().ContainKey("level");
        }

        [Fact]
        public void AIGraphNode_ShouldSupportBehaviorTree()
        {
            var leaf = new AINode("leaf1", AINodeKind.Leaf, action: new ActionNode("a1", "attack"));
            var bt = new AIGraphNode("bt1", AIGraphKind.BehaviorTree, rootChildId: "leaf1",
                new List<AINode> { leaf });
            bt.Kind.Should().Be(AIGraphKind.BehaviorTree);
            bt.Children.Should().HaveCount(1);
        }
    }
}
