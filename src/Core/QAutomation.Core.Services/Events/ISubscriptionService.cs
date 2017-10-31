using System.Collections.Generic;

namespace QAutomation.Core.Services.Events
{
    public interface ISubscriptionService
    {
        IEnumerable<IEventSubscriber<TEvent>> GetSyncedSubscriptions<TEvent>() where TEvent : EventBase;

        IEnumerable<IEventAsyncSubscriber<TEvent>> GetAsyncSubscriptions<TEvent>() where TEvent : EventBase;
    }
}