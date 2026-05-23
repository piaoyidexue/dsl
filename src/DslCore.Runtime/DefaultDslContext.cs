using System;
using System.Collections.Generic;
using DslCore.Abstractions;

namespace DslCore.Runtime
{
    public sealed class DefaultDslContext : IDslContext
    {
        private readonly Dictionary<string, object?> _variables = new Dictionary<string, object?>();
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        public object? GetVariable(string name) => _variables.TryGetValue(name, out var value) ? value : null;
        public void SetVariable(string name, object? value) => _variables[name] = value;
        public T GetService<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;
            throw new InvalidOperationException($"Service of type '{typeof(T).Name}' not registered");
        }
        public bool HasVariable(string name) => _variables.ContainsKey(name);
        public void RegisterService<T>(T service) where T : class => _services[typeof(T)] = service ?? throw new ArgumentNullException(nameof(service));
    }
}
