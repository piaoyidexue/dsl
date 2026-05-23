using DslCore.Abstractions;

namespace DslCore.AST
{
    public abstract class DslNode
    {
        public string Id { get; }
        public SourceLocation Location { get; }
        protected DslNode(string id, SourceLocation location = default)
        {
            Id = id ?? "";
            Location = location;
        }
    }
}
