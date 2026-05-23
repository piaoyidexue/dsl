using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;

namespace DslCore.Compiler.Validation
{
    public interface IValidationRule
    {
        string RuleId { get; }
        IEnumerable<DslError> Validate(RootNode ast, IValidationContext ctx);
    }
}
