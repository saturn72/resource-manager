using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;
using QAutomation.Core.Services.Events;
using QAutomation.Core.Services.Logging;
using QAutomation.Core.Domain.Logging;

namespace QAutomation.Core.Services.Tests.Events
{
    public class EventPublisherTests
    {
        [Fact]
        public void EventPublisher_ThrowsOnNullEvent()
        {
            var ep = new EventPublisher(null, null);
            Should.Throw<NullReferenceException>(() => ep.Publish((EventBase) null));
        }

        [Fact]
        public async Task EventPublisher_PublishToAsyncConsumers()
        {
            var asyncConsumer = new Mock<IEventAsyncSubscriber<DummyEvent>>();
            var subSrv = new Mock<ISubscriptionService>();
            subSrv.Setup(s => s.GetAsyncSubscriptions<DummyEvent>()).Returns(()=> new[] { asyncConsumer.Object });

            var ep = new EventPublisher(subSrv.Object, null);
            await Task.Run(()=> ep.PublishAsync(new DummyEvent()));
            Thread.Sleep(150);

            asyncConsumer.Verify(a => a.HandleEvent(It.IsAny<DummyEvent>()), Times.Once);
        }

        [Fact]
        public void EventPublisher_PublishToSyncedSubscribers()
        {
            var subSrv = new Mock<ISubscriptionService>();
            var syncedConsumer = new Mock<IEventSubscriber<DummyEvent>>();
            subSrv.Setup(s => s.GetSyncedSubscriptions<DummyEvent>()).Returns(new[] { syncedConsumer.Object });

            var ep = new EventPublisher(subSrv.Object, null);
            var eventMsg = new DummyEvent();
            ep.Publish(eventMsg);
            syncedConsumer.Verify(s => s.HandleEvent(It.IsAny<DummyEvent>()), Times.Once);
        }

        [Fact]
        public void EventPublisher_LogsExceptionOnsubscriberError()
        {
            var logRecords = new List<string>();

            var subSrv = new Mock<ISubscriptionService>();
            subSrv.Setup(s => s.GetAsyncSubscriptions<DummyEvent>()).Returns(new[] {new AsyncedThrowsSubscriber(),});
            subSrv.Setup(s => s.GetSyncedSubscriptions<DummyEvent>()).Returns(new[] {new SyncedThrowsSubscriber(),});

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(new[] {LogLevel.Error,});
            logger.Setup(l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<Guid>()))
                .Callback<LogLevel, string, string, Guid>((l, s1, s2, g) => logRecords.Add(s1));

            var ep = new EventPublisher(subSrv.Object, logger.Object);
            ep.Publish(new DummyEvent());

            Thread.Sleep(50);
            logRecords.Count.ShouldBe(1);
            logRecords.Any(m => m.Contains("-Asynced-")).ShouldBeFalse();
            logRecords.Any(m => m.Contains("-Synced-")).ShouldBeTrue();

        }

        public class AsyncedSubscriber : IEventAsyncSubscriber<DummyEvent>
        {
            public Task HandleEvent(DummyEvent eventMessage)
            {
                return new Task(() => eventMessage.AsyncedFlag = true);
            }
        }

        public class SyncedSubscriber : IEventSubscriber<DummyEvent>
        {
            public void HandleEvent(DummyEvent eventMessage)
            {
                eventMessage.SyncedFlag = true;
            }
        }

        public class AsyncedThrowsSubscriber : IEventAsyncSubscriber<DummyEvent>
        {
            public Task HandleEvent(DummyEvent eventMessage)
            {
                return new Task(() => { throw new NullReferenceException("-Asynced-Exception"); });
            }
        }

        public class SyncedThrowsSubscriber : IEventSubscriber<DummyEvent>
        {
            public void HandleEvent(DummyEvent eventMessage)
            {
                throw new NullReferenceException("-Synced-Exception");
            }
        }

    }
}