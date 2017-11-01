using LabManager.Common.Domain.Resource;
using QAutomation.Core.Domain;

namespace LabManager.Common.Domain.Runtime
{
    public class RuntimeSession:DomainModelBase
    {
        public string SessionId { get; set; }
        public ResourceModel Resource { get; set; }
    }
}
