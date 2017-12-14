using QAutomation.Core.Domain;

namespace QAutomation.Core.Services.Events
{
    public class DomainModelCreatedEvent<TDomainModel> : EventBase
        where TDomainModel : DomainModelBase
    {
        public TDomainModel Model { get; set; }
    }
}
