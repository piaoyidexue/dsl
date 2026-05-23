using System;
using System.Collections.Generic;
using DslCore.AST;

namespace DslCore.Compiler.Flow
{
    public sealed class CompiledFlowEdge
    {
        public string SourceNodeId { get; }
        public string TargetNodeId { get; }
        public EdgeKind Kind { get; }
        public Func<Abstractions.IDslContext, bool>? ConditionEvaluator { get; }
        public double Weight { get; }
        public string? TriggerEvent { get; }
        public CompiledFlowEdge(string sourceNodeId, string targetNodeId, EdgeKind kind, Func<Abstractions.IDslContext, bool>? conditionEvaluator = null, double weight = 1.0, string? triggerEvent = null)
        {
            SourceNodeId = sourceNodeId; TargetNodeId = targetNodeId; Kind = kind; ConditionEvaluator = conditionEvaluator; Weight = weight; TriggerEvent = triggerEvent;
        }
    }
}
