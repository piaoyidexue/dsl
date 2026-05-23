using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public sealed class ExpressionSyntaxRule : IValidationRule
    {
        public string RuleId => "DSL005";
        private readonly IExpressionEvaluator? _evaluator;
        public ExpressionSyntaxRule(IExpressionEvaluator? evaluator = null) => _evaluator = evaluator;
        public IEnumerable<DslError> Validate(RootNode ast, IValidationContext ctx)
        {
            var errors = new List<DslError>();
            foreach (var rule in ast.Rules)
                foreach (var cond in rule.Conditions)
                    ValidateCondition(cond, errors);
            foreach (var flow in ast.Flows)
                foreach (var edge in flow.Edges)
                    if (edge.Condition != null)
                        ValidateCondition(edge.Condition, errors);
            foreach (var sm in ast.StateMachines)
                foreach (var trans in sm.Transitions)
                    if (trans.Condition != null)
                        ValidateCondition(trans.Condition, errors);
            foreach (var evt in ast.Events)
                if (evt.Condition != null)
                    ValidateCondition(evt.Condition, errors);
            return errors;
        }
        private void ValidateCondition(ConditionNode cond, List<DslError> errors)
        {
            if (cond.Kind == ConditionKind.Expression && !string.IsNullOrEmpty(cond.Expression))
            {
                if (_evaluator != null)
                {
                    try { _evaluator.Evaluate(cond.Expression, new EmptyContext()); }
                    catch (System.Exception ex) { errors.Add(new DslError(DslErrorSeverity.Error, RuleId, $"Invalid expression: {ex.Message}", cond.Location)); }
                }
            }
            foreach (var child in cond.Children)
                ValidateCondition(child, errors);
        }
        private sealed class EmptyContext : IDslContext
        {
            public object? GetVariable(string name) => null;
            public void SetVariable(string name, object? value) { }
            public T GetService<T>() where T : class => throw new System.InvalidOperationException();
            public bool HasVariable(string name) => false;
        }
    }
}
