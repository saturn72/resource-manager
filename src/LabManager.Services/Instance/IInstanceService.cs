using System.Threading.Tasks;

namespace LabManager.Services.Instance
{
    public interface IInstanceService
    {
        Task<bool> Start(long resourceId);
    }
}
