using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.AST
{
    public enum ConditionKind
    {
        Expression,
        And,
        Or,
        Not
    }

    public sealed class ConditionNode : DslNode
    {
        public ConditionKind Kind { get; }
        public string? Expression { get; }
        public IReadOnlyList<ConditionNode> Children { get; }
        public ConditionNode(string id, string expression, SourceLocation location = default)
            : base(id, location)
        {
            Kind = ConditionKind.Expression;
            Expression = expression ?? "";
            Children = new List<ConditionNode>();
        }
        public ConditionNode(string id, ConditionKind kind, IReadOnlyList<ConditionNode> children, SourceLocation location = default)
            : base(id, location)
        {
            Kind = kind;
            Children = children ?? new List<ConditionNode>();
        }
    }
}
