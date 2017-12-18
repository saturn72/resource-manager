using LabManager.Common.Domain.Resource;

namespace LabManager.WebService.Models.Resources
{
    public class ResourceApiModel : ApiModelBase
    {
        public string FriendlyName { get; set; }
        public string IpAddress { get; set; }
        public ResourceStatus Status { get; set; }
        public bool Active { get; set; }
        public string SshUsername { get; set; }
        public string SshPassword { get; set; }
        public ushort SquishServerPort { get; set; }
        public string SquishServerLocalPath { get; set; }
        public string ObjectMapFilePath { get; set; }
    }
}