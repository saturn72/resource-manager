using LabManager.Common.Domain.Resource;

namespace LabManager.WebService.Models.Resources
{
    public class ResourceApiModel : ApiModelBase
    {
        public string FriendlyName { get; set; }
        public string IpAddress { get; set; }
        public ResourceStatus Status { get; set; }
    }
}