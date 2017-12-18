
using System.Collections.Generic;
using LabManager.Common.Domain.Resource;

namespace LabManager.Services.Runtime
{
    public class ResourceAssignmentRequest
    {
        private IEnumerable<ResourceModel> _requiredResources;

        public IEnumerable<ResourceModel> RequiredResources
        {
            get => _requiredResources ?? (_requiredResources = new List<ResourceModel>());
            set => _requiredResources = value;
        }

        public string ClientReferenceCode { get; set; }
        public int ResourceCount { get; set; }
    }
}
