using System;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
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

        #endregion

        #region ctor

        public InstanceService(IResourceRepository resourceRepository, ICacheManager cacheManager, AuditHelper auditHelper, IEventPublisher eventPublisher)
        {
            _resourceRepository = resourceRepository;
            _cacheManager = cacheManager;
            _auditHelper = auditHelper;
            _eventPublisher = eventPublisher;
        }

        #endregion

        public async Task<bool> Start(long resourceId)
        {
            if (resourceId <= 0)
                return false;

            var resourceByIdCacheKey = CacheKeyFormats.ResourceById.AsFormat(resourceId);
            var resource = _cacheManager.Get(resourceByIdCacheKey, ()=> _resourceRepository.GetById(resourceId), (int)CachingTimes.ResourceCachTime);
            if (resource.IsNull() || resource.Status == ResourceStatus.Unavailable)
                return false;

            var res = 1;
            await Task.Run(() =>
            {
                resource.Status = ResourceStatus.Started;
                _auditHelper.PrepareForUpdateAudity(resource);
                _resourceRepository.Update(resource);
                _eventPublisher.DomainModelUpdated(resource);

                //*************Runs startup scrpt here"
                res = 0;
                resource.Status = ResourceStatus.Available;
                _auditHelper.PrepareForUpdateAudity(resource);
                _resourceRepository.Update(resource);
                _eventPublisher.DomainModelUpdated(resource);
            });
            return res == 0;
        }
    }
}