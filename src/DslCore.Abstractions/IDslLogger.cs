using Microsoft.Extensions.Logging;

namespace DslCore.Abstractions
{
    public interface IDslLogger
    {
        void Log(DslErrorSeverity severity, string message, SourceLocation location = default);
        void LogError(string code, string message, SourceLocation location = default);
        void LogWarning(string code, string message, SourceLocation location = default);
    }
}
