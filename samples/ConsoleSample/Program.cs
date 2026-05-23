using System;
using System.Collections.Generic;
using DslCore.Abstractions;
using DslCore.AST;
using DslCore.Compiler;
using DslCore.Compiler.Expressions;
using DslCore.Compiler.Validation;
using DslCore.Runtime;
using DslCore.Serialization.Yaml;

namespace ConsoleSample
{
    public sealed class PrintActionHandler : IDslActionHandler
    {
        public bool CanHandle(string actionId) => true;
        public void Execute(string actionId, IReadOnlyDictionary<string, object?> parameters, IDslContext context)
        {
            Console.WriteLine($"  [Action] {actionId} executed");
            foreach (var kv in parameters)
                Console.WriteLine($"    {kv.Key} = {kv.Value}");
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var yaml = @"id: battle_flow
flows:
  - id: flow_main
    entry: start
    nodes:
      - id: start
        on_enter:
          - id: act_enter_start
            name: print
            params:
              message: Entering start node
        on_exit:
          - id: act_exit_start
            name: print
            params:
              message: Leaving start node
      - id: combat
        on_enter:
          - id: act_enter_combat
            name: print
            params:
              message: Entering combat node
        on_exit:
          - id: act_exit_combat
            name: print
            params:
              message: Leaving combat node
      - id: end
        on_enter:
          - id: act_enter_end
            name: print
            params:
              message: Entering end node - flow complete
    edges:
      - id: edge_start_combat
        from: start
        to: combat
        kind: unconditional
      - id: edge_combat_end
        from: combat
        to: end
        kind: event_triggered
        trigger_event: enemy_defeated
";

            Console.WriteLine("=== Step 1: Load YAML ===");
            var loader = new YamlDslLoader();
            var ast = loader.Load(yaml);
            Console.WriteLine($"AST loaded: id={ast.Id}, flows={ast.Flows?.Count ?? 0}");

            Console.WriteLine();
            Console.WriteLine("=== Step 2: Validate AST ===");
            using var evaluator = new JintExpressionEvaluator();
            var validator = DslValidator.CreateDefault(evaluator);
            var logger = DefaultDslLogger.Instance;
            var validationContext = new ValidationContext(logger);
            var validationResult = validator.Validate(ast, validationContext);
            Console.WriteLine($"Validation: IsValid={validationResult.IsValid}, Errors={validationResult.Errors.Count}");
            foreach (var err in validationResult.Errors)
                Console.WriteLine($"  {err}");

            Console.WriteLine();
            Console.WriteLine("=== Step 3: Compile ===");
            var compiler = new DslCompiler(evaluator);
            var compileResult = compiler.Compile(ast, validationContext);
            Console.WriteLine($"Compile: Success={compileResult.Success}");
            if (!compileResult.Success)
            {
                foreach (var err in compileResult.Errors)
                    Console.WriteLine($"  {err}");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("=== Step 4: Create VM and Run ===");
            var context = new DefaultDslContext();
            var dispatcher = new ActionDispatcher();
            dispatcher.RegisterGlobalHandler(new PrintActionHandler());

            var vm = new DslVirtualMachine(compileResult.Spec!, context, dispatcher);

            Console.WriteLine("--- StartFlow ---");
            var flow = vm.StartFlow("flow_main");
            Console.WriteLine($"Flow started: InstanceId={flow.InstanceId}, CurrentNode={flow.CurrentNodeId}, IsCompleted={flow.IsCompleted}");

            Console.WriteLine();
            Console.WriteLine("--- Tick (unconditional edge: start -> combat) ---");
            vm.Tick();
            Console.WriteLine($"After tick: CurrentNode={flow.CurrentNodeId}, IsCompleted={flow.IsCompleted}");

            Console.WriteLine();
            Console.WriteLine("--- PostEvent 'enemy_defeated' (event edge: combat -> end) ---");
            vm.PostEvent(new DslEvent("enemy_defeated"));
            Console.WriteLine($"After event: CurrentNode={flow.CurrentNodeId}, IsCompleted={flow.IsCompleted}");

            Console.WriteLine();
            Console.WriteLine("=== Done ===");
        }
    }
}
