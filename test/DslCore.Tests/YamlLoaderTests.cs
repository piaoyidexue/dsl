using DslCore.AST;
using DslCore.Serialization.Yaml;
using FluentAssertions;
using Xunit;

namespace DslCore.Tests
{
    public sealed class YamlLoaderTests
    {
        [Fact]
        public void Load_ShouldParseSimpleFlow()
        {
            var yaml = @"
id: test_root
flows:
  - id: flow1
    entry: start
    nodes:
      - id: start
      - id: end
    edges:
      - id: e1
        from: start
        to: end
";
            var loader = new YamlDslLoader();
            var root = loader.Load(yaml);
            root.Id.Should().Be("test_root");
            root.Flows.Should().HaveCount(1);
            root.Flows[0].Id.Should().Be("flow1");
            root.Flows[0].EntryNodeId.Should().Be("start");
            root.Flows[0].Nodes.Should().HaveCount(2);
            root.Flows[0].Edges.Should().HaveCount(1);
        }

        [Fact]
        public void Load_ShouldParseStateMachine()
        {
            var yaml = @"
id: sm_root
state_machines:
  - id: guard_ai
    initial: patrol
    states:
      - id: patrol
      - id: chase
      - id: attack
    transitions:
      - id: t1
        from: patrol
        to: chase
        trigger_event: enemy_spotted
      - id: t2
        from: chase
        to: attack
        condition:
          id: c1
          expression: distance < 5
";
            var loader = new YamlDslLoader();
            var root = loader.Load(yaml);
            root.StateMachines.Should().HaveCount(1);
            var sm = root.StateMachines[0];
            sm.Id.Should().Be("guard_ai");
            sm.InitialStateId.Should().Be("patrol");
            sm.States.Should().HaveCount(3);
            sm.Transitions.Should().HaveCount(2);
            sm.Transitions[1].Condition.Should().NotBeNull();
            sm.Transitions[1].Condition!.Kind.Should().Be(ConditionKind.Expression);
        }

        [Fact]
        public void Load_ShouldParseRules()
        {
            var yaml = @"
id: rule_root
rules:
  - id: damage_rule
    priority: 10
    conditions:
      - id: c1
        expression: hp <= 0
    actions:
      - id: a1
        name: trigger_death
";
            var loader = new YamlDslLoader();
            var root = loader.Load(yaml);
            root.Rules.Should().HaveCount(1);
            root.Rules[0].Priority.Should().Be(10);
            root.Rules[0].Conditions[0].Expression.Should().Be("hp <= 0");
            root.Rules[0].Actions[0].ActionName.Should().Be("trigger_death");
        }

        [Fact]
        public void Load_ShouldParseCompositeConditions()
        {
            var yaml = @"
id: comp_root
rules:
  - id: r1
    conditions:
      - id: and1
        kind: and
        children:
          - id: c1
            expression: x > 0
          - id: c2
            expression: y < 10
    actions: []
";
            var loader = new YamlDslLoader();
            var root = loader.Load(yaml);
            var cond = root.Rules[0].Conditions[0];
            cond.Kind.Should().Be(ConditionKind.And);
            cond.Children.Should().HaveCount(2);
        }

        [Fact]
        public void Load_NullInput_ShouldThrow()
        {
            var loader = new YamlDslLoader();
            var act = () => loader.Load(null!);
            act.Should().Throw<System.ArgumentNullException>();
        }
    }
}
