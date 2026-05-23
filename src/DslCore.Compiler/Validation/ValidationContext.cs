using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.Compiler.Validation
{
    public sealed class ValidationContext : IValidationContext
    {
        private readonly HashSet<string> _symbols;
        private readonly HashSet<string> _actions;
        public IDslLogger Logger { get; }
        public ValidationContext(IDslLogger logger, IEnumerable<string>? symbols = null, IEnumerable<string>? actions = null)
        {
            Logger = logger;
            _symbols = new HashSet<string>(symbols ?? new List<string>());
            _actions = new HashSet<string>(actions ?? new List<string>());
        }
        public bool SymbolExists(string id) => _symbols.Contains(id);
        public bool ActionExists(string actionId) => _actions.Contains(actionId);
        public void RegisterSymbol(string id) => _symbols.Add(id);
        public void RegisterAction(string actionId) => _actions.Add(actionId);
    }
}
