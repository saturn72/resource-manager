
using System.Collections.Generic;
using LabManager.Common.Domain.Resource;

namespace LabManager.Services.Runtime
{
    public class ResourceAssignmentRequest
    {
        public IEnumerable<ResourceModel> RequiredResources { get; set; }
        public string ClientReferenceCode { get; set; }
    }
}
