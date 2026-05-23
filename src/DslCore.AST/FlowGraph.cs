using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public sealed class FlowGraph : DslNode
    {
        public string? EntryNodeId { get; }
        public IReadOnlyList<FlowNode> Nodes { get; }
        public IReadOnlyList<FlowEdge> Edges { get; }
        public FlowGraph(string id, string? entryNodeId, IReadOnlyList<FlowNode>? nodes, IReadOnlyList<FlowEdge>? edges, SourceLocation location = default)
            : base(id, location)
        {
            EntryNodeId = entryNodeId;
            Nodes = nodes ?? new List<FlowNode>();
            Edges = edges ?? new List<FlowEdge>();
        }
    }
}
