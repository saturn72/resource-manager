using System;
using System.Collections.Generic;
using LabManager.Common.Domain.Resource;
using QAutomation.Core.Domain;

namespace LabManager.Services.Runtime
{
    public class ResourceAssignmentResponse:DomainModelBase
    {
        public ResourceAssignmentResponse(string sessionId, ResourceAssignmentRequest request)
        {
            SessionId = sessionId;
            Request = request;
            Status = ResourceAssignmentStatus.Pending;
        }

        public ResourceAssignmentRequest Request { get; }
        public string SessionId { get; }
        public IEnumerable<ResourceModel> Resources { get; set; }
        public ResourceAssignmentStatus Status { get; set; }
        public DateTime ExpiredOnUtc { get; set; }
        public DateTime? LastAccessedOnUtc { get; set; }
    }

    public enum ResourceAssignmentStatus
    {
        Pending = 10,
        Assigned = 20,
    }
}
