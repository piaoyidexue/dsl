using System;
using DslCore.Abstractions;

namespace DslCore.Runtime
{
    public sealed class DefaultDslLogger : IDslLogger
    {
        public static DefaultDslLogger Instance { get; } = new DefaultDslLogger();
        public void Log(DslErrorSeverity severity, string message, SourceLocation location = default)
        {
            var prefix = severity == DslErrorSeverity.Error ? "[ERROR]" : "[WARN]";
            Console.WriteLine($"{prefix} {message} @ {location}");
        }
        public void LogError(string code, string message, SourceLocation location = default)
        {
            Console.WriteLine($"[ERROR] {code}: {message} @ {location}");
        }
        public void LogWarning(string code, string message, SourceLocation location = default)
        {
            Console.WriteLine($"[WARN] {code}: {message} @ {location}");
        }
    }
}
