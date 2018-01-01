using LabManager.Common.Domain.Resource;

namespace LabManager.Services.Resources
{
    public interface IResourceController
    {
        int Start(ResourceModel resource);
        int Stop(ResourceModel resource);
        bool IsAlive(ResourceModel resource);
        void Command(string command);
    }
}
