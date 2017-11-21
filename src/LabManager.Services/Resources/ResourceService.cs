using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using QAutomation.Core.Services;
using QAutomation.Core.Services.Caching;
using QAutomation.Core.Services.Events;
using QAutomation.Extensions;

namespace LabManager.Services.Resources
{
    public class ResourceService : IResourceService
    {
        #region ctor

        public ResourceService(IResourceRepository resourceRepository, IEventPublisher eventPublisher, AuditHelper auditHelper, ICacheManager cacheManager)
        {
            _resourceRespository = resourceRepository;
            _eventPublisher = eventPublisher;
            _auditHelper = auditHelper;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Fields

        private readonly IResourceRepository _resourceRespository;
        private readonly IEventPublisher _eventPublisher;
        private readonly AuditHelper _auditHelper;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Create

        public Task<ServiceResponse<ResourceModel>> CreateAsync(ResourceModel model)
        {
            var srvRes = new ServiceResponse<ResourceModel>(model, ServiceRequestType.Create);
            ValidateModelForCreate(model, srvRes);
            if (srvRes.HasErrors())
                return Task.FromResult(srvRes);

            return Task.Run(() =>
            {
                _auditHelper.PrepareForCreateAudity(model);
                var id = _resourceRespository.Create(model);
                if (id == 0)
                    return null;
                srvRes.Model = _resourceRespository.GetById(id);
                srvRes.Result = ServiceResponseResult.Success;

                _eventPublisher.DomainModelCreated(model);

                return srvRes;
            });
        }

        private void ValidateModelForCreate(ResourceModel model, ServiceResponse<ResourceModel> serviceResponse)
        {
            if (model.IsNull())
            {
                serviceResponse.ErrorMessage = "Missing model data or null model.";
                serviceResponse.Result = ServiceResponseResult.Fail;
                return;
            }
            if (!model.FriendlyName.HasValue())
            {
                serviceResponse.ErrorMessage = "Resource Friendly name is required";
                serviceResponse.Result = ServiceResponseResult.Fail;
            }
            if (!_resourceRespository.GetBy(x => x.FriendlyName == model.FriendlyName && x.Active).IsEmptyOrNull())
            {
                serviceResponse.ErrorMessage = "Friendly name already exists";
                serviceResponse.Result = ServiceResponseResult.Fail;
            }

            if (!model.IpAddress.HasValue())
            {
                serviceResponse.ErrorMessage = "Resource IP Address is required.";
                serviceResponse.Result = ServiceResponseResult.Fail;
            }
        }

        #endregion

        #region Read

        public async Task<IEnumerable<ResourceModel>> GetAllAsync(ResourceModel filter = null)
        {
            Func<IEnumerable<ResourceModel>> query = () =>
                filter.IsNull()
                    ? _resourceRespository.GetAll()
                    : _resourceRespository.GetBy(filter);

            var resources = await Task.FromResult(query() ?? new ResourceModel[] { });
            foreach (var r in resources)
                _cacheManager.Set(CacheKeyFormats.ResourceById.AsFormat(r.Id), r);
            return resources;
        }

        public async Task<ResourceModel> GetById(long id)
        {
            return await Task.Run(() => _cacheManager.Get(CacheKeyFormats.ResourceById.AsFormat(id), 
                ()=> _resourceRespository.GetById(id)));
        }

        #region Update

        public async Task<ServiceResponse<ResourceModel>> UpdateAsync(ResourceModel resource)
        {
            var res = new ServiceResponse<ResourceModel>(resource, ServiceRequestType.Update);
            if (!ValidateModelForUpdate(resource, res))
                return res;

            return await Task.Run(()=>
            {
                var dbModel = PrepareForUpdate(resource);
                _resourceRespository.Update(dbModel);
                _cacheManager.Remove(CacheKeyFormats.ResourceById.AsFormat(dbModel.Id));
                res.Result = ServiceResponseResult.Success;

                return res;
            });
        }

        private ResourceModel PrepareForUpdate(ResourceModel resource)
        {
            var dbModel = _resourceRespository.GetById(resource.Id);
            if (resource.FriendlyName.HasValue())
                dbModel.FriendlyName = resource.FriendlyName;
            if (resource.IpAddress.HasValue())
                dbModel.IpAddress = resource.IpAddress;
            if (resource.SshUsername.HasValue())
                dbModel.SshUsername = resource.SshUsername;
            if (resource.SshPassword.HasValue())
                dbModel.SshPassword = resource.SshPassword;
            if(resource.SquishServerPort != default(ushort))
                dbModel.SquishServerPort = resource.SquishServerPort;
            dbModel.Active = resource.Active;

            _auditHelper.PrepareForUpdateAudity(dbModel);
            return dbModel;
        }

        private bool ValidateModelForUpdate(ResourceModel model, ServiceResponse<ResourceModel> serviceResponse)
        {
            if (model.IsNull() || model.Id == 0)
            {
                serviceResponse.ErrorMessage = "Missing model data or null model.";
                serviceResponse.Result = ServiceResponseResult.Fail;
            }

            return !serviceResponse.HasErrors();
        }
        #endregion

    }

    #endregion
}
