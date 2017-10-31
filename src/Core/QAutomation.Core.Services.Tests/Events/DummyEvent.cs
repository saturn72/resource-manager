using QAutomation.Core.Services.Events;

namespace QAutomation.Core.Services.Tests.Events
{
    public class DummyEvent : EventBase
    {
        public bool AsyncedFlag { get; set; }
        public bool SyncedFlag { get; set; }
    }
}