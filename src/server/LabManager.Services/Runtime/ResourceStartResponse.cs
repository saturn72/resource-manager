using Saturn72.Core.Domain;

namespace LabManager.Services.Runtime
{
    public class ResourceStartResponse : DomainModelBase
    {
        public ResourceStartResponse(long resourceId)
        {
            ResourceId = resourceId;
        }

        public long ResourceId { get; }
    }
}