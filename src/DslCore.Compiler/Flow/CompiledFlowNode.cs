using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.Compiler.Flow
{
    public sealed class CompiledFlowNode
    {
        public string Id { get; }
        public IReadOnlyList<DslCommand> OnEnterCommands { get; }
        public IReadOnlyList<DslCommand> OnExitCommands { get; }
        public CompiledFlowNode(string id, IReadOnlyList<DslCommand> onEnterCommands, IReadOnlyList<DslCommand> onExitCommands)
        {
            Id = id; OnEnterCommands = onEnterCommands; OnExitCommands = onExitCommands;
        }
    }
}
