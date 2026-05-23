using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.Compiler.StateMachine
{
    public sealed class CompiledState
    {
        public string Id { get; }
        public IReadOnlyList<DslCommand> OnEnterCommands { get; }
        public IReadOnlyList<DslCommand> OnExitCommands { get; }
        public CompiledState(string id, IReadOnlyList<DslCommand> onEnterCommands, IReadOnlyList<DslCommand> onExitCommands)
        {
            Id = id; OnEnterCommands = onEnterCommands; OnExitCommands = onExitCommands;
        }
    }
}
