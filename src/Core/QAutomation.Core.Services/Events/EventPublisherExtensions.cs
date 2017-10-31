using System.Threading.Tasks;
using QAutomation.Core.Domain;

namespace QAutomation.Core.Services.Events
{
    public static class EventPublisherExtensions
    {
        public static void DomainModelCreated<TDomainModel>(this IEventPublisher eventPublisher, TDomainModel model)
        where TDomainModel : DomainModelBase
        {
            var eventMessage = new DomainModelCreatedEvent<TDomainModel>
            {
                Model = model
            };
            eventPublisher.PublishAsync(eventMessage);
            Task.Run(() => eventPublisher.Publish(eventMessage));
        }
        public static void PublishToAll<TEvent>(IEventPublisher eventPublisher, TEvent eventMessage)
        where TEvent : EventBase
        {
            eventPublisher.PublishAsync(eventMessage);
            eventPublisher.Publish(eventMessage);
        }
    }
}