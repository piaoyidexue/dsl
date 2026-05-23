using System.Collections.Generic;
using System.Linq;
using DslCore.Abstractions;
using DslCore.AST;
using DslCore.Compiler.Expressions;
using DslCore.Compiler.Flow;
using DslCore.Compiler.StateMachine;
using DslCore.Compiler.Validation;

namespace DslCore.Compiler
{
    public sealed class DslCompiler
    {
        private readonly DslValidator _validator;
        private readonly FlowCompiler _flowCompiler;
        private readonly StateMachineCompiler _stateMachineCompiler;
        public DslCompiler(IExpressionEvaluator expressionEvaluator)
        {
            var exprCompiler = new ExpressionCompiler(expressionEvaluator);
            _validator = DslValidator.CreateDefault(expressionEvaluator);
            _flowCompiler = new FlowCompiler(exprCompiler);
            _stateMachineCompiler = new StateMachineCompiler(exprCompiler);
        }
        public CompileResult Compile(RootNode ast, IValidationContext validationContext)
        {
            var validationResult = _validator.Validate(ast, validationContext);
            if (!validationResult.IsValid)
                return CompileResult.CompileFailed(validationResult.Errors);
            var flowGraphs = ast.Flows.Select(f => _flowCompiler.Compile(f)).ToList();
            var stateMachines = ast.StateMachines.Select(sm => _stateMachineCompiler.Compile(sm)).ToList();
            var spec = new RuntimeSpec(flowGraphs, stateMachines);
            return CompileResult.CompileSuccess(spec, validationResult.Errors);
        }
    }
}
