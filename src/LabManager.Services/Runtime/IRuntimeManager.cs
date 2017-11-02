using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using LabManager.Common.Domain.Runtime;
using QAutomation.Core.Services;

namespace LabManager.Services.Runtime
{
    public interface IRuntimeManager
    {
        Task<ServiceResponse<RuntimeSession>> AssignResourceAsync(ResourceModel filter, bool availableOnly = true);
    }
}