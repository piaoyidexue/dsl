using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.Runtime;
using FluentAssertions;
using Xunit;

namespace DslCore.Tests
{
    public sealed class RuntimeTests
    {
        [Fact]
        public void DefaultDslContext_ShouldStoreVariables()
        {
            var ctx = new DefaultDslContext();
            ctx.HasVariable("x").Should().BeFalse();
            ctx.SetVariable("x", 42);
            ctx.HasVariable("x").Should().BeTrue();
            ctx.GetVariable("x").Should().Be(42);
        }

        [Fact]
        public void DefaultDslContext_ShouldStoreServices()
        {
            var ctx = new DefaultDslContext();
            var service = new TestService();
            ctx.RegisterService(service);
            ctx.GetService<TestService>().Should().BeSameAs(service);
        }

        [Fact]
        public void DefaultDslContext_GetService_WhenNotRegistered_ShouldThrow()
        {
            var ctx = new DefaultDslContext();
            var act = () => ctx.GetService<TestService>();
            act.Should().Throw<System.InvalidOperationException>();
        }

        [Fact]
        public void ActionDispatcher_ShouldDispatchToRegisteredHandler()
        {
            var dispatcher = new ActionDispatcher();
            var handler = new TrackingActionHandler();
            dispatcher.RegisterGlobalHandler(handler);
            var cmd = new DslCommand("action", "test_action", new Dictionary<string, object?>());
            dispatcher.Dispatch(cmd, new DefaultDslContext());
            handler.ExecutedActions.Should().Contain("test_action");
        }

        [Fact]
        public void DslCommand_ToString_ShouldFormat()
        {
            var cmd = new DslCommand("action", "spawn", new Dictionary<string, object?> { { "id", "goblin" } });
            cmd.ToString().Should().Contain("spawn");
            cmd.ToString().Should().Contain("goblin");
        }

        [Fact]
        public void DslEvent_ToString_ShouldFormat()
        {
            var evt = new DslEvent("player_died");
            evt.ToString().Should().Be("Event:player_died");
        }

        private sealed class TestService { }

        private sealed class TrackingActionHandler : IDslActionHandler
        {
            public List<string> ExecutedActions { get; } = new List<string>();
            public bool CanHandle(string actionId) => true;
            public void Execute(string actionId, IReadOnlyDictionary<string, object?> parameters, IDslContext context)
            {
                ExecutedActions.Add(actionId);
            }
        }
    }
}
