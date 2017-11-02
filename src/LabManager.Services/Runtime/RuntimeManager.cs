using System;
using System.Linq;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using LabManager.Common.Domain.Runtime;
using LabManager.Services.Resources;
using QAutomation.Core.Services;
using QAutomation.Extensions;

namespace LabManager.Services.Runtime
{
    public class RuntimeManager : IRuntimeManager
    {
        #region Fields

        private readonly IResourceService _resourceService;

        #endregion

        #region ctor

        public RuntimeManager(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        #endregion

        public async Task<ServiceResponse<RuntimeSession>> AssignResourceAsync(ResourceModel filter,
            bool availableOnly = true)
        {
            var srvRes = new ServiceResponse<RuntimeSession>(null, ServiceRequestType.Read);

            var resources = await _resourceService.GetAllAsync(filter);
            if (resources.IsEmptyOrNull() || (resources = resources.Where(r=>r.Active)).IsEmptyOrNull())
            {
                srvRes.Result = ServiceResponseResult.Fail;
                return srvRes;
            }

            srvRes.Model = CreateRuntimeSession();
            srvRes.Model.Resources = availableOnly
                ? resources.Where(r => GetResourceAvailability(r.Id) == ResourceStatus.Available).ToArray()
                : resources;
            srvRes.Result = ServiceResponseResult.Success;

            return srvRes;
        }

        private ResourceStatus GetResourceAvailability(long resourceId)
        {
            return ResourceStatus.Available;
        }

        private RuntimeSession CreateRuntimeSession()
        {
            return new RuntimeSession(Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));
        }
    }
}