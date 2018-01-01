using System.Threading.Tasks;
using Saturn72.Core.Services;

namespace LabManager.Services.Instance
{
    public interface IInstanceService
    {
        Task<ServiceResponse<ResourceExecutionResponseData>> StartAsync(long resourceId);
        Task<ServiceResponse<ResourceExecutionResponseData>> StopAsync(long resourceId);
    }
}
