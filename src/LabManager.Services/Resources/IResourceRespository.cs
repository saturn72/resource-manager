using System.Collections.Generic;
using LabManager.Common.Domain.Resource;

namespace LabManager.Services.Resources
{
    public interface IResourceRepository
    {
        IEnumerable<ResourceModel> GetAll();
        long Create(ResourceModel resourceModel);
        ResourceModel GetById(long id);
    }
}