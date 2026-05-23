using System;
using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;
using DslCore.Compiler.Expressions;

namespace DslCore.Compiler.StateMachine
{
    public sealed class StateMachineCompiler
    {
        private readonly ExpressionCompiler _expressionCompiler;
        public StateMachineCompiler(ExpressionCompiler expressionCompiler) => _expressionCompiler = expressionCompiler;
        public CompiledStateMachine Compile(StateMachineNode sm)
        {
            var states = sm.States.ToDictionary(
                s => s.Id,
                s => new CompiledState(
                    s.Id,
                    s.OnEnterActions.Select(a => new DslCommand("action", a.ActionName, a.Parameters, a.Location)).ToList(),
                    s.OnExitActions.Select(a => new DslCommand("action", a.ActionName, a.Parameters, a.Location)).ToList()));
            var transitions = sm.Transitions.Select(t =>
            {
                Func<IDslContext, bool>? condEval = null;
                if (t.Condition != null)
                    condEval = CompileCondition(t.Condition);
                var cmds = t.Actions.Select(a => new DslCommand("action", a.ActionName, a.Parameters, a.Location)).ToList();
                return new CompiledTransition(t.Id, t.SourceStateId, t.TargetStateId, condEval, t.TriggerEvent, cmds);
            }).ToList();
            return new CompiledStateMachine(sm.Id, sm.InitialStateId, states, transitions);
        }
        private Func<IDslContext, bool> CompileCondition(ConditionNode condition)
        {
            switch (condition.Kind)
            {
                case ConditionKind.Expression:
                    var compiled = _expressionCompiler.Compile(condition.Expression!);
                    return ctx => IsTruthy(compiled(ctx));
                case ConditionKind.And:
                    var andChildren = condition.Children.Select(CompileCondition).ToList();
                    return ctx => andChildren.All(f => f(ctx));
                case ConditionKind.Or:
                    var orChildren = condition.Children.Select(CompileCondition).ToList();
                    return ctx => orChildren.Any(f => f(ctx));
                case ConditionKind.Not:
                    if (condition.Children.Count > 0)
                    {
                        var inner = CompileCondition(condition.Children[0]);
                        return ctx => !inner(ctx);
                    }
                    return ctx => false;
                default:
                    return ctx => false;
            }
        }
        private static bool IsTruthy(object? value)
        {
            if (value == null) return false;
            if (value is bool b) return b;
            if (value is int i) return i != 0;
            if (value is double d) return d != 0.0;
            if (value is string s) return !string.IsNullOrEmpty(s);
            return true;
        }
    }
}
