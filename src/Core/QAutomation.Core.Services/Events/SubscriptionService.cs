using System;
using System.Collections.Generic;

namespace QAutomation.Core.Services.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        public IEnumerable<IEventSubscriber<TEvent>> GetSyncedSubscriptions<TEvent>() where TEvent : EventBase
        {
            return new IEventSubscriber<TEvent>[]{} ;
        }
        public IEnumerable<IEventAsyncSubscriber<TEvent>> GetAsyncSubscriptions<TEvent>() where TEvent : EventBase
        {
            return new IEventAsyncSubscriber<TEvent>[]{} ;
        }
    }
}