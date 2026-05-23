using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;

namespace DslCore.Compiler.Validation
{
    public sealed class ValidationResult
    {
        public IReadOnlyList<DslError> Errors { get; }
        public bool HasErrors => Errors.Any(e => e.Severity == DslErrorSeverity.Error);
        public bool HasWarnings => Errors.Any(e => e.Severity == DslErrorSeverity.Warning);
        public bool IsValid => !HasErrors;
        public ValidationResult(IEnumerable<DslError> errors) => Errors = (errors ?? new List<DslError>()).ToList();
    }
}
