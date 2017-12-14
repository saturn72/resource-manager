using System.Collections.Generic;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using Saturn72.Core.Services;

namespace LabManager.Services.Resources
{
    public interface IResourceService
    {
        Task<IEnumerable<ResourceModel>> GetAllAsync(ResourceModel filter = null);
        Task<ServiceResponse<ResourceModel>> CreateAsync(ResourceModel model);
        Task<ResourceModel> GetById(long id);
        Task<ServiceResponse<ResourceModel>> UpdateAsync(ResourceModel resource);
    }
}