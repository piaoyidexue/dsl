namespace DslCore.Abstractions
{
    public sealed class DslError
    {
        public DslErrorSeverity Severity { get; }
        public string Code { get; }
        public string Message { get; }
        public SourceLocation Location { get; }
        public DslError(DslErrorSeverity severity, string code, string message, SourceLocation location = default)
        {
            Severity = severity; Code = code ?? ""; Message = message ?? ""; Location = location;
        }
        public override string ToString() => $"[{Severity}] {Code}: {Message} @ {Location}";
    }
}
