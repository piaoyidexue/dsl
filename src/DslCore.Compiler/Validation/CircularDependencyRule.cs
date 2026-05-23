using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public sealed class CircularDependencyRule : IValidationRule
    {
        public string RuleId => "DSL004";
        public IEnumerable<DslError> Validate(RootNode ast, IValidationContext ctx)
        {
            var errors = new List<DslError>();
            foreach (var flow in ast.Flows)
            {
                var adjacency = new Dictionary<string, List<string>>();
                foreach (var edge in flow.Edges)
                {
                    if (!adjacency.TryGetValue(edge.SourceNodeId, out var list))
                    {
                        list = new List<string>();
                        adjacency[edge.SourceNodeId] = list;
                    }
                    list.Add(edge.TargetNodeId);
                }
                var visited = new HashSet<string>();
                var stack = new HashSet<string>();
                foreach (var node in flow.Nodes)
                    if (!visited.Contains(node.Id))
                        DetectCycle(node.Id, adjacency, visited, stack, errors);
            }
            return errors;
        }
        private void DetectCycle(string nodeId, Dictionary<string, List<string>> adj, HashSet<string> visited, HashSet<string> stack, List<DslError> errors)
        {
            visited.Add(nodeId);
            stack.Add(nodeId);
            if (adj.TryGetValue(nodeId, out var neighbors))
                foreach (var next in neighbors)
                {
                    if (!visited.Contains(next))
                        DetectCycle(next, adj, visited, stack, errors);
                    else if (stack.Contains(next))
                        errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Circular dependency detected involving node '{next}'", SourceLocation.Unknown));
                }
            stack.Remove(nodeId);
        }
    }
}
