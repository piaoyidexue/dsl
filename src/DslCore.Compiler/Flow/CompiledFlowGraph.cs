using System.Collections.Generic;

namespace DslCore.Compiler.Flow
{
    public sealed class CompiledFlowGraph
    {
        public string Id { get; }
        public string? EntryNodeId { get; }
        public IReadOnlyDictionary<string, CompiledFlowNode> Nodes { get; }
        public IReadOnlyList<CompiledFlowEdge> Edges { get; }
        public CompiledFlowGraph(string id, string? entryNodeId, IReadOnlyDictionary<string, CompiledFlowNode> nodes, IReadOnlyList<CompiledFlowEdge> edges)
        {
            Id = id; EntryNodeId = entryNodeId; Nodes = nodes; Edges = edges;
        }
    }
}
