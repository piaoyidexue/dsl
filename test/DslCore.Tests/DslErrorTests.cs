using DslCore.Abstractions;
using FluentAssertions;
using Xunit;

namespace DslCore.Tests
{
    public sealed class DslErrorTests
    {
        [Fact]
        public void Constructor_ShouldSetProperties()
        {
            var loc = new SourceLocation("f.dsl", 1, 1);
            var error = new DslError(DslErrorSeverity.Error, "E001", "test error", loc);
            error.Severity.Should().Be(DslErrorSeverity.Error);
            error.Code.Should().Be("E001");
            error.Message.Should().Be("test error");
            error.Location.Should().Be(loc);
        }

        [Fact]
        public void ToString_ShouldIncludeAllInfo()
        {
            var error = new DslError(DslErrorSeverity.Warning, "W001", "warn", new SourceLocation("a.dsl", 5, 3));
            error.ToString().Should().Contain("Warning");
            error.ToString().Should().Contain("W001");
            error.ToString().Should().Contain("warn");
        }
    }
}
