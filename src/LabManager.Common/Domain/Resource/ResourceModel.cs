using System;
using QAutomation.Core.Audit;
using QAutomation.Core.Domain;

namespace LabManager.Common.Domain.Resource
{
    public class ResourceModel : DomainModelBase, IFullAudit
    {
        public string FriendlyName { get; set; }
        public string IpAddress { get; set; }
        public ResourceStatus Status { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public long? DeletedByUserId { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public long? UpdatedByUserId { get; set; }
        public string SshUsername { get; set; }
        public string SshPassword { get; set; }
    }
}