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

        #region RequestResourceAssignmentAsync
        public async Task<ServiceResponse<ResourceAssignmentResponse>> RequestResourceAssignmentAsync(ResourceAssignmentRequest assignRequest, bool availableOnly = true)
        {
            var srvRes = new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Read);

            var requestedResource = await GetRequestedResources(assignRequest);
            ValidateRequestedResources(requestedResource, srvRes, assignRequest);
            if (srvRes.HasErrors())
                return srvRes;

            srvRes.Model = CreateResourceAssignmentResponse(assignRequest);
            srvRes.Model.Resources = availableOnly
                ? requestedResource.Where(r => GetResourceAvailability(r.Id) == ResourceStatus.Available).ToArray()
                : requestedResource;
            srvRes.Result = ServiceResponseResult.Success;
            throw new NotImplementedException("Insert resources to cache");
            return srvRes;
        }

        private void ValidateRequestedResources(IEnumerable<ResourceModel> requestedResource, ServiceResponse<ResourceAssignmentResponse> serviceResponse, ResourceAssignmentRequest assignRequest)
        {
            if (requestedResource.IsEmptyOrNull())
            {
                serviceResponse.ErrorMessage = "No matching resources found";
                serviceResponse.Result = ServiceResponseResult.Fail;
                return;
            }

            if(requestedResource.Count()<assignRequest.RequiredResources.Count())
            {
                serviceResponse.ErrorMessage = "Insufficient Resources";
                serviceResponse.Result = ServiceResponseResult.Fail;
                return;
            }
        }

        private async Task<IEnumerable<ResourceModel>> GetRequestedResources(ResourceAssignmentRequest assignRequest)
        {
            var allResources = new List<ResourceModel>();
            foreach (var rr in assignRequest.RequiredResources)
            {
                var range = await _resourceService.GetAllAsync(rr);
                if (range.IsEmptyOrNull())
                    continue;
                allResources.AddRange(range);
            }
            return allResources?.Distinct().Where(r=>r.Active);
        }

#endregion
        public Task<ServiceResponse<ResourceAssignmentResponse>> AssignResourcesAsync(string sessionId)
        {
            var srvRes = new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Approve);
            if (!sessionId.HasValue())
            {
                srvRes.ErrorMessage = "The specified session-id is empty";
                return Task.FromResult(srvRes);
            }
            var resources = _cacheManager.Get<object>(sessionId);
            if (resources.IsNull())
            {
                srvRes.ErrorMessage = "Session expired";
                return Task.FromResult(srvRes);
            }

            //var key = string.Format(SessionCacheKeyFormat, sessionId);
            //_cacheManager.Get<
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