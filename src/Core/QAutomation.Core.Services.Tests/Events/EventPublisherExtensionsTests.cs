using Moq;
using QAutomation.Core.Services.Events;
using Xunit;

namespace QAutomation.Core.Services.Tests.Events
{
    public class EventPublisherExtensionsTests
    {
        [Fact]
        public void EventExtensionsTests_PublishToAll()
        {
            var ep = new Mock<IEventPublisher>();
            EventPublisherExtensions.PublishToAll(ep.Object, new DummyEvent());

            ep.Verify(e=>e.Publish(It.IsAny<DummyEvent>()), Times.Once);
            ep.Verify(e=>e.PublishAsync(It.IsAny<DummyEvent>()), Times.Once);

        }
    }
}
