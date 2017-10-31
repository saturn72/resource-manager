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
        #region Fields
        private readonly IResourceRepository _resourceRespository;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        #region ctor
        public ResourceService(IResourceRepository resourceRepository, IEventPublisher eventPublisher)
        {
            _resourceRespository = resourceRepository;
            _eventPublisher = eventPublisher;
        }

        public Task<ServiceResponse<ResourceModel>> CreateAsync(ResourceModel model)
        {
            var srvRes = new ServiceResponse<ResourceModel> (model, ServiceRequestType.Create);
            ValidateModelForCreate(model, srvRes);
            if(srvRes.HasErrors())
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
                serviceResponse.ErrorMessage = "Resource Friendly name is required.";
                serviceResponse.Result = ServiceResponseResult.Fail;
            }
            if (!model.IpAddress.HasValue())
            {
                serviceResponse.ErrorMessage = "Resource IP Address is required.";
                serviceResponse.Result = ServiceResponseResult.Fail;
            }
        }

        #endregion
        public Task<IEnumerable<ResourceModel>> GetAllAsync()
        {
            return Task.Run(() => _resourceRespository.GetAll() ?? new ResourceModel[] { });
        }

        public Task<ResourceModel> GetById(long id)
        {
            return Task.Run(() => _resourceRespository.GetById(id));
        }
    }
}