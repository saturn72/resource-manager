using QAutomation.Core.Domain;

namespace LabManager.Common.Domain.Resource
{
    public class ResourceModel : DomainModelBase
    {
        public string FriendlyName { get; set; }
        public string IpAddress { get; set; }
        public ResourceStatus Status { get; set; }
    }
}