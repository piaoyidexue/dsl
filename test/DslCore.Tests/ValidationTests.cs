using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;
using DslCore.Compiler.Validation;
using FluentAssertions;
using Xunit;

namespace DslCore.Tests
{
    public sealed class ValidationTests
    {
        private static IValidationContext CreateContext()
        {
            return new ValidationContext(DefaultTestLogger.Instance);
        }

        [Fact]
        public void IdUniquenessRule_ShouldDetectDuplicates()
        {
            var rule = new IdUniquenessRule();
            var node1 = new FlowNode("dup");
            var node2 = new FlowNode("dup");
            var flow = new FlowGraph("f1", "dup",
                new List<FlowNode> { node1, node2 },
                new List<FlowEdge>());
            var root = new RootNode("root", flows: new List<FlowGraph> { flow });
            var errors = rule.Validate(root, CreateContext());
            errors.Should().Contain(e => e.Code == "DSL001");
        }

        [Fact]
        public void IdUniquenessRule_ShouldPassWithUniqueIds()
        {
            var rule = new IdUniquenessRule();
            var node1 = new FlowNode("n1");
            var node2 = new FlowNode("n2");
            var flow = new FlowGraph("f1", "n1",
                new List<FlowNode> { node1, node2 },
                new List<FlowEdge>());
            var root = new RootNode("root", flows: new List<FlowGraph> { flow });
            var errors = rule.Validate(root, CreateContext());
            errors.Should().BeEmpty();
        }

        [Fact]
        public void ReferenceExistenceRule_ShouldDetectMissingEntryNode()
        {
            var rule = new ReferenceExistenceRule();
            var flow = new FlowGraph("f1", "nonexistent",
                new List<FlowNode> { new FlowNode("n1") },
                new List<FlowEdge>());
            var root = new RootNode("root", flows: new List<FlowGraph> { flow });
            var errors = rule.Validate(root, CreateContext());
            errors.Should().Contain(e => e.Code == "DSL002" && e.Message.Contains("nonexistent"));
        }

        [Fact]
        public void ReferenceExistenceRule_ShouldDetectMissingEdgeTarget()
        {
            var rule = new ReferenceExistenceRule();
            var edge = new FlowEdge("e1", "n1", "missing");
            var flow = new FlowGraph("f1", "n1",
                new List<FlowNode> { new FlowNode("n1") },
                new List<FlowEdge> { edge });
            var root = new RootNode("root", flows: new List<FlowGraph> { flow });
            var errors = rule.Validate(root, CreateContext());
            errors.Should().Contain(e => e.Code == "DSL002" && e.Message.Contains("missing"));
        }

        [Fact]
        public void FlowReachabilityRule_ShouldDetectUnreachableNodes()
        {
            var rule = new FlowReachabilityRule();
            var n1 = new FlowNode("n1");
            var n2 = new FlowNode("n2");
            var n3 = new FlowNode("n3");
            var edge = new FlowEdge("e1", "n1", "n2");
            var flow = new FlowGraph("f1", "n1",
                new List<FlowNode> { n1, n2, n3 },
                new List<FlowEdge> { edge });
            var root = new RootNode("root", flows: new List<FlowGraph> { flow });
            var errors = rule.Validate(root, CreateContext());
            errors.Should().Contain(e => e.Code == "DSL003" && e.Message.Contains("n3"));
        }

        [Fact]
        public void DslValidator_CreateDefault_ShouldIncludeAllRules()
        {
            var validator = DslValidator.CreateDefault();
            var root = new RootNode("root");
            var result = validator.Validate(root, CreateContext());
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidationResult_ShouldDetectErrors()
        {
            var errors = new List<DslError>
            {
                new DslError(DslErrorSeverity.Error, "E001", "error"),
                new DslError(DslErrorSeverity.Warning, "W001", "warning")
            };
            var result = new ValidationResult(errors);
            result.HasErrors.Should().BeTrue();
            result.HasWarnings.Should().BeTrue();
            result.IsValid.Should().BeFalse();
        }

        private sealed class DefaultTestLogger : IDslLogger
        {
            public static DefaultTestLogger Instance { get; } = new DefaultTestLogger();
            public void Log(DslErrorSeverity severity, string message, SourceLocation location = default) { }
            public void LogError(string code, string message, SourceLocation location = default) { }
            public void LogWarning(string code, string message, SourceLocation location = default) { }
        }
    }
}
