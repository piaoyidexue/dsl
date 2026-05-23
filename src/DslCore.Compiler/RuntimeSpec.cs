using System.Collections.Generic;
using DslCore.Compiler.Flow;
using DslCore.Compiler.StateMachine;

namespace DslCore.Compiler
{
    public sealed class RuntimeSpec
    {
        public IReadOnlyList<CompiledFlowGraph> FlowGraphs { get; }
        public IReadOnlyList<CompiledStateMachine> StateMachines { get; }
        public RuntimeSpec(IReadOnlyList<CompiledFlowGraph>? flowGraphs, IReadOnlyList<CompiledStateMachine>? stateMachines)
        {
            FlowGraphs = flowGraphs ?? new List<CompiledFlowGraph>();
            StateMachines = stateMachines ?? new List<CompiledStateMachine>();
        }
    }
}
