using System.Collections.Generic;
using LabManager.WebService.Models.Resources;

namespace LabManager.WebService.Models.Runtime
{
    public class ResourceAssignmentRequestApiModel
    {
        public IEnumerable<ResourceApiModel> RequiredResources { get; set; }
        public string ClientReferenceCode { get; set; }
    }
}
