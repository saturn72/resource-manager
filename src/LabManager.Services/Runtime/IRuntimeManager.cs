using System.Threading.Tasks;
using Saturn72.Core.Services;

namespace LabManager.Services.Runtime
{
    public interface IRuntimeManager
    {
        Task<ServiceResponse<ResourceAssignmentResponse>> RequestResourceAssignmentAsync(ResourceAssignmentRequest assignRequest, bool availableOnly = true);
        Task<ServiceResponse<ResourceAssignmentResponse>> AssignResourcesAsync(string sessionId);
        Task<bool> IsAssigned(long resourceId);
    }
}