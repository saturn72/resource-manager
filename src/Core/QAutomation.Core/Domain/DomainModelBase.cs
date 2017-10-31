namespace QAutomation.Core.Domain
{
    public abstract class DomainModelBase<TId>
    {
        public TId Id { get; set; }
    }

    public class DomainModelBase : DomainModelBase<long>
    {

    }
}