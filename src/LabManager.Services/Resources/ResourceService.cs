using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabManager.Common.Domain.Resource;
using QAutomation.Core.Services;
using QAutomation.Core.Services.Events;
using QAutomation.Extensions;

namespace LabManager.Services.Resources
{
    public class ResourceService : IResourceService
    {
        #region ctor

        public ResourceService(IResourceRepository resourceRepository, IEventPublisher eventPublisher)
        {
            _resourceRespository = resourceRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Fields

        private readonly IResourceRepository _resourceRespository;
        private readonly IEventPublisher _eventPublisher;

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
            if(!_resourceRespository.GetBy(x=>x.FriendlyName == model.FriendlyName && x.Active).IsEmptyOrNull())
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

            return await Task.FromResult(query() ?? new ResourceModel[] { });
        }

        public async Task<ResourceModel> GetById(long id)
        {
            return await Task.Run(() => _resourceRespository.GetById(id));
        }

        #endregion
    }
}