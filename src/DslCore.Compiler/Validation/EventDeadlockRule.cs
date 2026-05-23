using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public sealed class EventDeadlockRule : IValidationRule
    {
        public string RuleId => "DSL006";
        public IEnumerable<DslError> Validate(RootNode ast, IValidationContext ctx)
        {
            var errors = new List<DslError>();
            var eventWaitNodes = new Dictionary<string, List<string>>();
            foreach (var flow in ast.Flows)
                foreach (var edge in flow.Edges)
                    if (edge.Kind == EdgeKind.EventTriggered && edge.TriggerEvent != null)
                    {
                        if (!eventWaitNodes.TryGetValue(edge.TriggerEvent, out var list))
                        {
                            list = new List<string>();
                            eventWaitNodes[edge.TriggerEvent] = list;
                        }
                        list.Add(edge.SourceNodeId);
                    }
            var eventTypes = new HashSet<string>(ast.Events.Select(e => e.EventType));
            foreach (var kvp in eventWaitNodes)
                if (!eventTypes.Contains(kvp.Key))
                    errors.Add(new DslError(DslErrorSeverity.Warning, RuleId, $"Flow waits for event '{kvp.Key}' which is never defined", SourceLocation.Unknown));
            return errors;
        }
    }
}
