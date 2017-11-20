using System.Threading.Tasks;
using QAutomation.Core.Services;

namespace LabManager.Services.Instance
{
    public interface IInstanceService
    {
        Task<ServiceResponse<ResourceExecutionResponseData>> StartAsync(long resourceId);
        Task<ServiceResponse<ResourceExecutionResponseData>> StopAsync(long resourceId);
    }
}
