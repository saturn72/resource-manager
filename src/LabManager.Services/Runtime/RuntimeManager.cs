using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using LabManager.Services.Resources;
using Saturn72.Core.Caching;
using Saturn72.Core.Services;
using Saturn72.Extensions;

namespace LabManager.Services.Runtime
{
    public class RuntimeManager : IRuntimeManager
    {
        private const int Settings_ResourceManagement_ResourceAvilabilityTime_InSeconds = 100;

        #region Fields

        private readonly IResourceService _resourceService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region ctor

        public RuntimeManager(IResourceService resourceService, ICacheManager cacheManager)
        {
            _resourceService = resourceService;
            _cacheManager = cacheManager;
        }

        #endregion

        #region RequestResourceAssignmentAsync
        public async Task<ServiceResponse<ResourceAssignmentResponse>> RequestResourceAssignmentAsync(ResourceAssignmentRequest assignRequest, bool availableOnly = true)
        {
            var srvRes = new ServiceResponse<ResourceAssignmentResponse>(ServiceRequestType.Read);

            var requestedResource = await GetRequestedResources(assignRequest);
            ValidateRequestedResources(requestedResource, srvRes, assignRequest);
            if (srvRes.HasErrors())
                return srvRes;

            srvRes.Model = CreateResourceAssignmentResponse(assignRequest);
            srvRes.Model.Resources = availableOnly
                ? requestedResource.Where(r => GetResourceAvailability(r.Id) == ResourceStatus.Available).ToArray()
                : requestedResource;
            srvRes.Result = ServiceResponseResult.Success;
            _cacheManager.Set(srvRes.Model.SessionId, srvRes.Model);
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

            if (requestedResource.Count() < assignRequest.RequiredResources.Count())
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
            return allResources?.Distinct().Where(r => r.Active);
        }

        #endregion
        public async Task<ServiceResponse<ResourceAssignmentResponse>> AssignResourcesAsync(string sessionId)
        {
            var srvRes = new ServiceResponse<ResourceAssignmentResponse>(ServiceRequestType.Approve){Model = null};
            if (!sessionId.HasValue())
            {
                srvRes.ErrorMessage = "The specified session-id is empty";
                srvRes.Result = ServiceResponseResult.Fail;
                return srvRes;
            }
            var resAssignResponse = _cacheManager.Get<ResourceAssignmentResponse>(sessionId);
            if (resAssignResponse.IsNull() || resAssignResponse.Resources.IsEmptyOrNull())
            {
                srvRes.ErrorMessage = "Session expired";
                srvRes.Result = ServiceResponseResult.Fail;
                return srvRes;
            }

            foreach (var resource in resAssignResponse.Resources)
            {
                resource.Status = ResourceStatus.Assigned;
                await _resourceService.UpdateAsync(resource);
            }
            resAssignResponse.Status = ResourceAssignmentStatus.Assigned;
            //TODO: save to persistancy layer here
            srvRes.Model = resAssignResponse;
            srvRes.Result = ServiceResponseResult.Success;
            return srvRes;
        }

        public Task<bool> IsAssigned(long resourceId)
        {

            throw new NotImplementedException("Save current status to DB was not implemented");
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