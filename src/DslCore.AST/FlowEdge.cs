using DslCore.Abstractions;

namespace DslCore.AST
{
    public enum EdgeKind
    {
        Unconditional,
        Conditional,
        Weighted,
        EventTriggered
    }

    public sealed class FlowEdge : DslNode
    {
        public string SourceNodeId { get; }
        public string TargetNodeId { get; }
        public EdgeKind Kind { get; }
        public ConditionNode? Condition { get; }
        public double Weight { get; }
        public string? TriggerEvent { get; }
        public FlowEdge(string id, string sourceNodeId, string targetNodeId, EdgeKind kind = EdgeKind.Unconditional, ConditionNode? condition = null, double weight = 1.0, string? triggerEvent = null, SourceLocation location = default)
            : base(id, location)
        {
            SourceNodeId = sourceNodeId ?? "";
            TargetNodeId = targetNodeId ?? "";
            Kind = kind;
            Condition = condition;
            Weight = weight;
            TriggerEvent = triggerEvent;
        }
    }
}
