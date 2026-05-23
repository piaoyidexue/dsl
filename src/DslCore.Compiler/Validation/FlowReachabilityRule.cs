using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public sealed class FlowReachabilityRule : IValidationRule
    {
        public string RuleId => "DSL003";
        public IEnumerable<DslError> Validate(RootNode ast, IValidationContext ctx)
        {
            var errors = new List<DslError>();
            foreach (var flow in ast.Flows)
            {
                if (flow.EntryNodeId == null) continue;
                var reachable = new HashSet<string>();
                var adjacency = BuildAdjacency(flow);
                var queue = new Queue<string>();
                queue.Enqueue(flow.EntryNodeId);
                reachable.Add(flow.EntryNodeId);
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    if (adjacency.TryGetValue(current, out var neighbors))
                        foreach (var next in neighbors)
                            if (reachable.Add(next))
                                queue.Enqueue(next);
                }
                foreach (var node in flow.Nodes)
                    if (!reachable.Contains(node.Id))
                        errors.Add(new DslError(DslErrorSeverity.Warning, RuleId, $"Unreachable flow node: '{node.Id}'", node.Location));
            }
            return errors;
        }
        private static Dictionary<string, List<string>> BuildAdjacency(FlowGraph flow)
        {
            var adj = new Dictionary<string, List<string>>();
            foreach (var edge in flow.Edges)
            {
                if (!adj.TryGetValue(edge.SourceNodeId, out var list))
                {
                    list = new List<string>();
                    adj[edge.SourceNodeId] = list;
                }
                list.Add(edge.TargetNodeId);
            }
            return adj;
        }
    }
}
