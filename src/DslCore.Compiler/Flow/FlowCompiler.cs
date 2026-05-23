using System;
using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;
using DslCore.Compiler.Expressions;

namespace DslCore.Compiler.Flow
{
    public sealed class FlowCompiler
    {
        private readonly ExpressionCompiler _expressionCompiler;
        public FlowCompiler(ExpressionCompiler expressionCompiler) => _expressionCompiler = expressionCompiler;
        public CompiledFlowGraph Compile(FlowGraph flowGraph)
        {
            var nodes = flowGraph.Nodes.ToDictionary(
                n => n.Id,
                n => new CompiledFlowNode(
                    n.Id,
                    n.OnEnterActions.Select(a => new DslCommand("action", a.ActionName, a.Parameters, a.Location)).ToList(),
                    n.OnExitActions.Select(a => new DslCommand("action", a.ActionName, a.Parameters, a.Location)).ToList()));
            var edges = flowGraph.Edges.Select(e =>
            {
                Func<IDslContext, bool>? condEval = null;
                if (e.Condition != null)
                {
                    var compiledCond = CompileCondition(e.Condition);
                    condEval = compiledCond;
                }
                return new CompiledFlowEdge(e.SourceNodeId, e.TargetNodeId, e.Kind, condEval, e.Weight, e.TriggerEvent);
            }).ToList();
            return new CompiledFlowGraph(flowGraph.Id, flowGraph.EntryNodeId, nodes, edges);
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
