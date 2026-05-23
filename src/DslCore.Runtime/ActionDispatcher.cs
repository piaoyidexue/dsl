using System;
using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.Runtime
{
    public sealed class ActionDispatcher
    {
        private readonly Dictionary<string, IDslActionHandler> _handlers = new Dictionary<string, IDslActionHandler>();
        private readonly List<IDslActionHandler> _globalHandlers = new List<IDslActionHandler>();
        public void RegisterHandler(string actionId, IDslActionHandler handler)
        {
            _handlers[actionId] = handler ?? throw new ArgumentNullException(nameof(handler));
        }
        public void RegisterGlobalHandler(IDslActionHandler handler)
        {
            _globalHandlers.Add(handler ?? throw new ArgumentNullException(nameof(handler)));
        }
        public void Dispatch(DslCommand command, IDslContext context)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (_handlers.TryGetValue(command.TargetId, out var handler))
            {
                handler.Execute(command.TargetId, command.Parameters, context);
                return;
            }
            foreach (var globalHandler in _globalHandlers)
            {
                if (globalHandler.CanHandle(command.TargetId))
                {
                    globalHandler.Execute(command.TargetId, command.Parameters, context);
                    return;
                }
            }
        }
        public void DispatchAll(IEnumerable<DslCommand> commands, IDslContext context)
        {
            foreach (var cmd in commands)
                Dispatch(cmd, context);
        }
    }
}
