using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public sealed class DslValidator
    {
        private readonly List<IValidationRule> _rules = new List<IValidationRule>();
        public DslValidator() { }
        public DslValidator(IEnumerable<IValidationRule> rules) => _rules.AddRange(rules);
        public void AddRule(IValidationRule rule) => _rules.Add(rule);
        public static DslValidator CreateDefault(IExpressionEvaluator? evaluator = null)
        {
            var validator = new DslValidator();
            validator.AddRule(new IdUniquenessRule());
            validator.AddRule(new ReferenceExistenceRule());
            validator.AddRule(new FlowReachabilityRule());
            validator.AddRule(new CircularDependencyRule());
            validator.AddRule(new ExpressionSyntaxRule(evaluator));
            validator.AddRule(new EventDeadlockRule());
            return validator;
        }
        public ValidationResult Validate(RootNode ast, IValidationContext ctx)
        {
            var errors = new List<DslError>();
            foreach (var rule in _rules)
                errors.AddRange(rule.Validate(ast, ctx));
            return new ValidationResult(errors);
        }
    }
}
