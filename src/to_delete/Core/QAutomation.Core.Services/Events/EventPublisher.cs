using System;
using System.Diagnostics;
using System.Threading.Tasks;
using QAutomation.Extensions;
using QAutomation.Core.Services.Logging;

namespace QAutomation.Core.Services.Events
{
    public class EventPublisher : IEventPublisher
    {
        private static readonly ParallelOptions ParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };

        #region Fields

        private readonly ILogger _logger;
        private readonly ISubscriptionService _subscriptionService;

        #endregion
        #region contructor
        public EventPublisher(ISubscriptionService subscriptionService, ILogger logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }
        #endregion
        public void Publish<TEvent>(TEvent eventMessage) where TEvent : EventBase
        {
            Guard.NotNull(eventMessage);
            //synced subscribers
            var subscriptions = _subscriptionService.GetSyncedSubscriptions<TEvent>();
            subscriptions.ForEachItem(x => PublishSyncedToConsumer(() => x.HandleEvent(eventMessage)));
        }

        public void PublishAsync<TEvent>(TEvent eventMessage) where TEvent : EventBase
        {
            Guard.NotNull(eventMessage);

            //Async subscribers
            //NOTE: IIS might "fold" the applicaiton pool when threads are in middle of execution
            var asyncSubscribers = _subscriptionService.GetAsyncSubscriptions<TEvent>();

            Parallel.ForEach(asyncSubscribers, ParallelOptions, a => PublishAsyncedToConsumer(a.HandleEvent(eventMessage)));
        }

        protected virtual void PublishAsyncedToConsumer(Task task)
        {
            Action<Task> onAsyncExceptionAction = t =>
            {
                var ex = t.Exception;
                if (ex.IsNull())
                    return;

                LogException(ex);
            };

            task.ContinueWith(onAsyncExceptionAction, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
        protected virtual void PublishSyncedToConsumer(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void LogException(Exception ex)
        {
            //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
            try
            {
                _logger.Error(ex.Message, ex);
                var innerException = ex.InnerException;

                if (innerException.NotNull())
                    _logger.Error(innerException.Message, innerException);
            }
            catch (Exception)
            {
                Trace.TraceError("EventPublisher ==> Failed to write to log");
            }
        }
    }
}