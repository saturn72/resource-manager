using System;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using LabManager.Services.Commanders;
using LabManager.Services.Resources;
using QAutomation.Core.Services;
using QAutomation.Core.Services.Caching;
using QAutomation.Core.Services.Events;
using QAutomation.Extensions;

namespace LabManager.Services.Instance
{
    public class InstanceService : IInstanceService
    {
        #region Fields

        private readonly IResourceRepository _resourceRepository;
        private readonly ICacheManager _cacheManager;
        private readonly AuditHelper _auditHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IResourceCommander _resourceCommander;

        #endregion

        #region ctor

        public InstanceService(IResourceRepository resourceRepository, ICacheManager cacheManager, AuditHelper auditHelper, IEventPublisher eventPublisher, IResourceCommander resourceCommander)
        {
            _resourceRepository = resourceRepository;
            _cacheManager = cacheManager;
            _auditHelper = auditHelper;
            _eventPublisher = eventPublisher;
            _resourceCommander = resourceCommander;
        }

        #endregion

        public async Task<ServiceResponse<ResourceExecutionResponseData>> StartAsync(long resourceId)
        {
            return await RunResourceCommand(resourceId, resource => _resourceCommander.Start(resource), ResourceStatus.Started);
        }

        public async Task<ServiceResponse<ResourceExecutionResponseData>> StopAsync(long resourceId)
        {
            return await RunResourceCommand(resourceId, resource => _resourceCommander.Stop(resource), ResourceStatus.Stopped);
        }

        #region Utilities

        private async Task<ServiceResponse<ResourceExecutionResponseData>> RunResourceCommand(long resourceId, Func<ResourceModel, int> resourceCommand, ResourceStatus resourceStatus)
        {
            var response = new ServiceResponse<ResourceExecutionResponseData>(new ResourceExecutionResponseData(), ServiceRequestType.Command);
            if (resourceId <= 0)
            {
                response.ErrorMessage = "Illegal resource Id";
                response.Result = ServiceResponseResult.Fail;
                return response;
            }

            var resourceByIdCacheKey = CacheKeyFormats.ResourceById.AsFormat(resourceId);
            var resource = _cacheManager.Get(resourceByIdCacheKey, () => _resourceRepository.GetById(resourceId), (int)CachingTimes.ResourceCachTime);
            if (!CheckResourceBeproStartup(resource, response))
                return response;

            var result = 0;
            await Task.Run(() =>
            {
                resource.Status = resourceStatus;
                _auditHelper.PrepareForUpdateAudity(resource);
                _resourceRepository.Update(resource);
                _eventPublisher.DomainModelUpdated(resource);

                result = resourceCommand(resource);

                resource.Status = ResourceStatus.Available;
                _auditHelper.PrepareForUpdateAudity(resource);
                _resourceRepository.Update(resource);
                _eventPublisher.DomainModelUpdated(resource);

            });

            response.Model.Executed = result == 0;
            response.Result = response.Model.Executed ?
                ServiceResponseResult.Success :
                ServiceResponseResult.Fail;

            return response;
        }

        private bool CheckResourceBeproStartup(ResourceModel resource, ServiceResponse<ResourceExecutionResponseData> response)
        {
            if (resource.IsNull())
            {
                response.ErrorMessage = "Resource not found";
                response.Result = ServiceResponseResult.Fail;
                return false;
            }
            if (resource.Status == ResourceStatus.Unavailable)
                response.ErrorMessage = "Resource unavailable";

            response.Result = response.ErrorMessage.IsEmptyOrNull()
                ? ServiceResponseResult.Success
                : ServiceResponseResult.Fail;
            return response.Result == ServiceResponseResult.Success;
        }
        #endregion
    }
}