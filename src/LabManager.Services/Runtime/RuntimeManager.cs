using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using LabManager.Services.Resources;
using QAutomation.Core.Services;
using QAutomation.Extensions;

namespace LabManager.Services.Runtime
{
    public class RuntimeManager : IRuntimeManager
    {
        private const int Settings_ResourceManagement_ResourceAvilabilityTime_InSeconds = 100;

        #region Fields

        private readonly IResourceService _resourceService;

        #endregion

        #region ctor

        public RuntimeManager(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        #endregion

        public async Task<ServiceResponse<ResourceAssignmentResponse>> RequestResourceAssignmentAsync(ResourceAssignmentRequest assignRequest, bool availableOnly = true)
        {
            var srvRes = new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Read);

            var allResources = new List<ResourceModel>();
            foreach (var rr in assignRequest.RequiredResources)
            {
                var range = await _resourceService.GetAllAsync(rr);
                if(range.IsEmptyOrNull())
                    continue;
                allResources.AddRange(range);
            }

            var resources = allResources?.Distinct();

            if (resources.IsEmptyOrNull() || (resources = resources.Where(r => r.Active)).IsEmptyOrNull())
            {
                srvRes.Result = ServiceResponseResult.Fail;
                return srvRes;
            }

            srvRes.Model = CreateResourceAssignmentResponse(assignRequest);
            srvRes.Model.Resources = availableOnly
                ? resources.Where(r => GetResourceAvailability(r.Id) == ResourceStatus.Available).ToArray()
                : resources;
            srvRes.Result = ServiceResponseResult.Success;

            return srvRes;
        }

        public Task<ServiceResponse<ResourceAssignmentResponse>> AssignResourcesAsync(string sessionId)
        {
            throw new NotImplementedException();
        }

        private ResourceStatus GetResourceAvailability(long resourceId)
        {
            return ResourceStatus.Available;
        }

        private ResourceAssignmentResponse CreateResourceAssignmentResponse(ResourceAssignmentRequest assignRequest)
        {
            var sessionId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
            var rs = new ResourceAssignmentResponse(sessionId, assignRequest);
            rs.ExpiredOnUtc = DateTime.UtcNow.AddSeconds(Settings_ResourceManagement_ResourceAvilabilityTime_InSeconds);
            return rs;
        }
    }
}