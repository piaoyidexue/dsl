using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public sealed class IdUniquenessRule : IValidationRule
    {
        public string RuleId => "DSL001";
        public IEnumerable<DslError> Validate(RootNode ast, IValidationContext ctx)
        {
            var seen = new HashSet<string>();
            var errors = new List<DslError>();
            CollectIds(ast, seen, errors);
            return errors;
        }
        private void CollectIds(RootNode root, HashSet<string> seen, List<DslError> errors)
        {
            CheckNode(root.Id, root.Location, seen, errors);
            foreach (var rule in root.Rules) CheckNode(rule.Id, rule.Location, seen, errors);
            foreach (var flow in root.Flows)
            {
                CheckNode(flow.Id, flow.Location, seen, errors);
                foreach (var node in flow.Nodes) CheckNode(node.Id, node.Location, seen, errors);
                foreach (var edge in flow.Edges) CheckNode(edge.Id, edge.Location, seen, errors);
            }
            foreach (var sm in root.StateMachines)
            {
                CheckNode(sm.Id, sm.Location, seen, errors);
                foreach (var state in sm.States) CheckNode(state.Id, state.Location, seen, errors);
                foreach (var trans in sm.Transitions) CheckNode(trans.Id, trans.Location, seen, errors);
            }
            foreach (var evt in root.Events) CheckNode(evt.Id, evt.Location, seen, errors);
            foreach (var ai in root.AIGraphs) CheckNode(ai.Id, ai.Location, seen, errors);
            foreach (var spawn in root.SpawnConfigs) CheckNode(spawn.Id, spawn.Location, seen, errors);
        }
        private void CheckNode(string id, SourceLocation loc, HashSet<string> seen, List<DslError> errors)
        {
            if (!seen.Add(id))
                errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Duplicate id: '{id}'", loc));
        }
    }
}
