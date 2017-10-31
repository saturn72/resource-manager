using System;

namespace QAutomation.Core.Services.Events
{
    public abstract class EventBase
    {
        protected EventBase()
        {
            FiredOnUtc = DateTime.UtcNow;
        }

        public DateTime FiredOnUtc { get; protected set; }
    }
}