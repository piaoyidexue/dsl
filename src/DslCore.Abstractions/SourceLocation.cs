using System;

namespace DslCore.Abstractions
{
    public readonly struct SourceLocation : IEquatable<SourceLocation>
    {
        public string FileName { get; }
        public int Line { get; }
        public int Column { get; }
        public SourceLocation(string fileName, int line, int column) { FileName = fileName ?? ""; Line = line; Column = column; }
        public static SourceLocation Unknown => new SourceLocation("", 0, 0);
        public bool Equals(SourceLocation other) => FileName == other.FileName && Line == other.Line && Column == other.Column;
        public override bool Equals(object? obj) => obj is SourceLocation other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(FileName, Line, Column);
        public override string ToString() => string.IsNullOrEmpty(FileName) ? $"({Line}:{Column})" : $"{FileName}({Line}:{Column})";
        public static bool operator ==(SourceLocation left, SourceLocation right) => left.Equals(right);
        public static bool operator !=(SourceLocation left, SourceLocation right) => !left.Equals(right);
    }
}
