using System.Collections.Generic;

namespace DslCore.Abstractions
{
    public readonly struct DslEvent
    {
        public string EventType { get; }
        public IReadOnlyDictionary<string, object?> Data { get; }
        public DslEvent(string eventType, IReadOnlyDictionary<string, object?>? data = null)
        {
            EventType = eventType ?? ""; Data = data ?? new Dictionary<string, object?>();
        }
        public override string ToString() => $"Event:{EventType}";
    }
}
