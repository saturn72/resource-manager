using System.Threading.Tasks;
using QAutomation.Core.Services;

namespace LabManager.Services.Runtime
{
    public interface IRuntimeManager
    {
        Task<ServiceResponse<ResourceAssignmentResponse>> RequestResourceAssignmentAsync(ResourceAssignmentRequest assignRequest, bool availableOnly = true);
        Task<ServiceResponse<ResourceAssignmentResponse>> AssignResourcesAsync(string sessionId);
    }
}