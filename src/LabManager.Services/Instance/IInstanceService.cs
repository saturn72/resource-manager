using System.Threading.Tasks;
using QAutomation.Core.Services;

namespace LabManager.Services.Instance
{
    public interface IInstanceService
    {
        Task<ServiceResponse<StartResponseData>> Start(long resourceId);
    }
}
