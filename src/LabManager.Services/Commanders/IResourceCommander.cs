using LabManager.Common.Domain.Resource;

namespace LabManager.Services.Commanders
{
    public interface IResourceCommander
    {
        int Start(ResourceModel resource);
        int Stop(ResourceModel resource);
        bool IsAlive(ResourceModel resource);
    }
}
