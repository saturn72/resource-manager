using System.Threading.Tasks;

namespace QAutomation.Core.Services.Events
{
    public interface IEventAsyncSubscriber<in TEvent> where TEvent : EventBase
    {
        Task HandleEvent(TEvent eventMessage);
    }
}