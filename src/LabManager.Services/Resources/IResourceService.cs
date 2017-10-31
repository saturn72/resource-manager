using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using QAutomation.Core.Services;

namespace LabManager.Services.Resources
{
    public interface IResourceService
    {
        Task<IEnumerable<ResourceModel>> GetAllAsync();
        Task<ServiceResponse<ResourceModel>> CreateAsync(ResourceModel model);
        Task<ResourceModel> GetById(long id);
    }
}