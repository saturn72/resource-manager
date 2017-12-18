using System.Collections.Generic;
using LabManager.WebService.Models.Resources;

namespace LabManager.WebService.Models.Runtime
{
    public class ResourceAssignmentRequestApiModel
    {
        public ResourceAssignmentRequestApiModel()
        {
            ResourceCount = 1;
        }
        public int ResourceCount { get; set; }
        public IEnumerable<ResourceApiModel> Filter { get; set; }
        public string ClientReferenceCode { get; set; }
    }
}
