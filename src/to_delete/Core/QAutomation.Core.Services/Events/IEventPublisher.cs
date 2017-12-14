namespace QAutomation.Core.Services.Events
{
    public interface IEventPublisher
    {
        void PublishAsync<TEvent>(TEvent eventMessage) where TEvent : EventBase;
        void Publish<TEvent>(TEvent eventMessage) where TEvent : EventBase;
    }
}