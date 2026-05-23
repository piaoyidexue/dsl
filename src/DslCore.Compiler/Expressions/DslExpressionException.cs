using System;

namespace DslCore.Compiler.Expressions
{
    public sealed class DslExpressionException : Exception
    {
        public DslExpressionException(string message) : base(message) { }
        public DslExpressionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
