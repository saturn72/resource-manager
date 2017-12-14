
namespace QAutomation.Core.Services.Events
{
    public interface IEventSubscriber<in TEvent> where TEvent : EventBase
    {
        void HandleEvent(TEvent eventMessage);
    }
}