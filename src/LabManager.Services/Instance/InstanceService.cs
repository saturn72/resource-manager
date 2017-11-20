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

        public async Task<ServiceResponse<StartResponseData>> Start(long resourceId)
        {
            var data = new StartResponseData();
            var response = new ServiceResponse<StartResponseData>(data, ServiceRequestType.Launch);
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

            var result = 1;
            await Task.Run(() =>
            {
                resource.Status = ResourceStatus.Started;
                _auditHelper.PrepareForUpdateAudity(resource);
                _resourceRepository.Update(resource);
                _eventPublisher.DomainModelUpdated(resource);

                result = _resourceCommander.Start(resource);

                resource.Status = ResourceStatus.Available;
                _auditHelper.PrepareForUpdateAudity(resource);
                _resourceRepository.Update(resource);
                _eventPublisher.DomainModelUpdated(resource);
                
            });

            response.Model.Started = result == 0;
            response.Result = response.Model.Started?
                ServiceResponseResult.Success : 
                ServiceResponseResult.Fail;

            return response;
        }

        private bool CheckResourceBeproStartup(ResourceModel resource, ServiceResponse<StartResponseData> response)
        {
            if (resource.IsNull())
            {
                response.ErrorMessage = "Resource not found";
                response.Result = ServiceResponseResult.Fail;
                return false;
            }
            if (resource.Status == ResourceStatus.Unavailable)
                response.ErrorMessage = "Resource unavailable";

            if (resource.IpAddress.IsEmptyOrNull() || resource.SshUsername.IsEmptyOrNull() ||
                resource.SshPassword.IsEmptyOrNull())
                response.ErrorMessage = "Missing resource data for ssh connection";

            response.Result = response.ErrorMessage.IsEmptyOrNull()
                ? ServiceResponseResult.Success
                : ServiceResponseResult.Fail;
            return response.Result == ServiceResponseResult.Success;
        }

       
    }
}