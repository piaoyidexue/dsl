using DslCore.Abstractions;
using FluentAssertions;
using Xunit;

namespace DslCore.Tests
{
    public sealed class SourceLocationTests
    {
        [Fact]
        public void Unknown_ShouldHaveEmptyFileName()
        {
            SourceLocation.Unknown.FileName.Should().BeEmpty();
            SourceLocation.Unknown.Line.Should().Be(0);
            SourceLocation.Unknown.Column.Should().Be(0);
        }

        [Fact]
        public void Equals_ShouldCompareAllFields()
        {
            var a = new SourceLocation("test.dsl", 1, 5);
            var b = new SourceLocation("test.dsl", 1, 5);
            var c = new SourceLocation("other.dsl", 1, 5);
            a.Should().Be(b);
            a.Should().NotBe(c);
        }

        [Fact]
        public void Operators_ShouldWork()
        {
            var a = new SourceLocation("f", 1, 1);
            var b = new SourceLocation("f", 1, 1);
            (a == b).Should().BeTrue();
            (a != b).Should().BeFalse();
        }

        [Fact]
        public void ToString_ShouldFormatCorrectly()
        {
            var loc = new SourceLocation("test.dsl", 10, 20);
            loc.ToString().Should().Be("test.dsl(10:20)");
            SourceLocation.Unknown.ToString().Should().Be("(0:0)");
        }
    }
}
