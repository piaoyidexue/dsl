using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;

namespace DslCore.Compiler
{
    public sealed class CompileResult
    {
        public bool Success { get; }
        public RuntimeSpec? Spec { get; }
        public IReadOnlyList<DslError> Errors { get; }
        private CompileResult(bool success, RuntimeSpec? spec, IReadOnlyList<DslError> errors)
        {
            Success = success; Spec = spec; Errors = errors;
        }
        public static CompileResult CompileFailed(IEnumerable<DslError> errors) => new CompileResult(false, null, errors.ToList());
        public static CompileResult CompileSuccess(RuntimeSpec spec, IEnumerable<DslError> warnings) => new CompileResult(true, spec, warnings.Where(e => e.Severity == DslErrorSeverity.Warning).ToList());
    }
}
