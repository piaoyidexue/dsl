using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public sealed class ReferenceExistenceRule : IValidationRule
    {
        public string RuleId => "DSL002";
        public IEnumerable<DslError> Validate(RootNode ast, IValidationContext ctx)
        {
            var errors = new List<DslError>();
            var nodeIds = CollectAllNodeIds(ast);
            foreach (var flow in ast.Flows)
            {
                if (flow.EntryNodeId != null && !nodeIds.Contains(flow.EntryNodeId))
                    errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Entry node '{flow.EntryNodeId}' not found", flow.Location));
                foreach (var edge in flow.Edges)
                {
                    if (!nodeIds.Contains(edge.SourceNodeId))
                        errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Source node '{edge.SourceNodeId}' not found", edge.Location));
                    if (!nodeIds.Contains(edge.TargetNodeId))
                        errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Target node '{edge.TargetNodeId}' not found", edge.Location));
                }
            }
            foreach (var sm in ast.StateMachines)
            {
                var stateIds = new HashSet<string>(sm.States.Select(s => s.Id));
                if (sm.InitialStateId != null && !stateIds.Contains(sm.InitialStateId))
                    errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Initial state '{sm.InitialStateId}' not found", sm.Location));
                foreach (var trans in sm.Transitions)
                {
                    if (!stateIds.Contains(trans.SourceStateId))
                        errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Source state '{trans.SourceStateId}' not found", trans.Location));
                    if (!stateIds.Contains(trans.TargetStateId))
                        errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Target state '{trans.TargetStateId}' not found", trans.Location));
                }
            }
            foreach (var spawn in ast.SpawnConfigs)
            {
                if (!ctx.SymbolExists(spawn.EntityId))
                    errors.Add(new DslError(DslErrorSeverity.Warning, RuleId, $"Entity '{spawn.EntityId}' not registered", spawn.Location));
            }
            return errors;
        }

        private static HashSet<string> CollectAllNodeIds(RootNode ast)
        {
            var ids = new HashSet<string>();
            foreach (var flow in ast.Flows)
                foreach (var node in flow.Nodes)
                    ids.Add(node.Id);
            return ids;
        }
    }
}
