using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using QAutomation.Core.Services.Events;
using LabManager.Services;
using LabManager.Services.Resources;
using LabManager.Common.Domain.Resource;
using System.Linq;
using Shouldly;
using QAutomation.Core.Services;
using QAutomation.Core;
using QAutomation.Core.Services.Caching;

namespace LabManager.Services.Tests.Resource
{
    public class ResourceServiceTests
    {
        #region Fields

        private readonly Mock<IResourceRepository> _resourceRepository;
        private readonly Mock<IEventPublisher> _eventPublisher;
        private readonly Mock<AuditHelper> _auditHelper;
        private readonly Mock<ICacheManager> _cacheManager;

        private readonly ResourceService _resourceService;

        #endregion

        #region ctor

        public ResourceServiceTests()
        {
            _resourceRepository = new Mock<IResourceRepository>();
            _eventPublisher = new Mock<IEventPublisher>();

            var wc = new Mock<IWorkContext>();
            wc.Setup(w => w.ClientId).Returns("client-id");
            wc.Setup(w => w.CurrentUserId).Returns(123);
            wc.Setup(w => w.CurrentUserIpAddress).Returns("ip-address");
            _auditHelper = new Mock<AuditHelper>(wc.Object);

            _cacheManager = new Mock<ICacheManager>();

            _resourceService = new ResourceService(_resourceRepository.Object, _eventPublisher.Object,
                _auditHelper.Object,
                _cacheManager.Object);
        }

        #endregion

        #region GetAll
        [Fact]
        public async Task ResourceService_GetAllAsync_Fails()
        {
            //return null
            _resourceRepository.Setup(r => r.GetAll()).Returns(null as IEnumerable<ResourceModel>);
            (await _resourceService.GetAllAsync()).Count().ShouldBe(0);

            //returns empty collection
            _resourceRepository.Setup(r => r.GetAll()).Returns(new ResourceModel[] { } as IEnumerable<ResourceModel>);
            (await _resourceService.GetAllAsync()).Count().ShouldBe(0);
        }

        [Fact]
        public async Task ResourceService_GetAllAsync_NoFilter_Passes()
        {
            var expRes = new ResourceModel[]{
                new ResourceModel{Id = 1, FriendlyName = "fn1"},
                new ResourceModel{Id = 2, FriendlyName = "fn2"},
                new ResourceModel{Id = 3, FriendlyName = "fn3"},
                new ResourceModel{Id = 4, FriendlyName = "fn4"},
            };

            _resourceRepository.Setup(r => r.GetAll()).Returns(expRes as IEnumerable<ResourceModel>);
            var col = (await _resourceService.GetAllAsync());
            col.Count().ShouldBe(expRes.Length);
            var names = expRes.Select(s => s.FriendlyName);

            foreach (var c in col)
            {
                names.ShouldContain(c.FriendlyName);
                _cacheManager.Verify(cm => cm.Set(It.Is<string>(k => k == string.Format(CacheKeyFormats.ResourceById, c.Id))
                , It.IsAny<ResourceModel>(),
                It.IsAny<int>()), Times.Once);
            }
        }

        [Fact]
        public async Task ResourceService_GetAllAsync_Filter_Passes()
        {
            var expRes = new ResourceModel[]{
                new ResourceModel{Id = 1, FriendlyName = "fn1"},
                new ResourceModel{Id = 2,FriendlyName = "fn2"},
                new ResourceModel{Id = 3,FriendlyName = "fn3"},
                new ResourceModel{Id = 4,FriendlyName = "fn4"},
            };

            _resourceRepository.Setup(r => r.GetBy(It.IsAny<Func<ResourceModel, bool>>())).Returns(expRes as IEnumerable<ResourceModel>);
            var col = (await _resourceService.GetAllAsync(new ResourceModel
            {
                FriendlyName = "fn1"
            }));
            col.Count().ShouldBe(expRes.Length);
            var names = expRes.Select(s => s.FriendlyName);

            foreach (var c in col)
            {
                names.ShouldContain(c.FriendlyName);
                _cacheManager.Verify(cm => cm.Set(It.Is<string>(k => k == string.Format(CacheKeyFormats.ResourceById, c.Id))
                    , It.IsAny<ResourceModel>(),
                    It.IsAny<int>()), Times.Once);
            }
        }
        #endregion

        #region Create
        [Fact]
        public async Task ResourceService_Add_Fails()
        {
            ResourceModel toCreate = null;
            var actual = await _resourceService.CreateAsync(toCreate);

            actual.Model.ShouldBeNull();
            actual.HasErrors().ShouldBeTrue();
            actual.Result.ShouldBe(ServiceResponseResult.Fail);
            actual.RequestType.ShouldBe(ServiceRequestType.Create);

            toCreate = new ResourceModel { };
            actual = await _resourceService.CreateAsync(toCreate);

            actual.Model.ShouldNotBeNull();
            actual.HasErrors().ShouldBeTrue();
            actual.Result.ShouldBe(ServiceResponseResult.Fail);
            actual.RequestType.ShouldBe(ServiceRequestType.Create);

            //friendly name already exists
            toCreate = new ResourceModel { FriendlyName = "duplicate-name" };
            _resourceRepository.Setup(r => r.GetBy(It.IsAny<Func<ResourceModel, bool>>())).Returns(new[] { new ResourceModel() });
            actual = await _resourceService.CreateAsync(toCreate);

            actual.Model.ShouldNotBeNull();
            actual.HasErrors().ShouldBeTrue();
            actual.Result.ShouldBe(ServiceResponseResult.Fail);
            actual.RequestType.ShouldBe(ServiceRequestType.Create);
        }

        [Fact]
        public async Task ResourceService_Add()
        {
            const string fName = "fn1";
            var toCreate = new ResourceModel
            {
                FriendlyName = fName,
                IpAddress = "192.168.2.2"
            };
            var createdId = (long)123;

            _resourceRepository.Setup(r => r.Create(It.IsAny<ResourceModel>())).Returns(createdId);
            _resourceRepository.Setup(r => r.GetById(It.IsAny<long>())).Returns(() =>
            {
                toCreate.Id = createdId;
                return toCreate;
            });
            var actual = await _resourceService.CreateAsync(toCreate);

            actual.Result.ShouldBe(ServiceResponseResult.Success);
            actual.Model.FriendlyName.ShouldBe(fName);
            actual.Model.Id.ShouldBe(createdId);

            //verify audity
            _auditHelper.Verify(a => a.PrepareForCreateAudity(It.IsAny<ResourceModel>()), Times.Once);
            //verify event was raised
            _eventPublisher.Verify(e => e.PublishAsync(It.IsAny<DomainModelCreatedEvent<ResourceModel>>()), Times.Once);
            _eventPublisher.Verify(e => e.Publish(It.IsAny<DomainModelCreatedEvent<ResourceModel>>()), Times.Once);
        }
        #endregion

        #region GetById

        [Fact]
        public async Task ResourceService_GetById_ReturnsNull()
        {
            _resourceRepository.Setup(r => r.GetById(It.IsAny<long>())).Returns(null as ResourceModel);
            var actual = await _resourceService.GetById(123);

            actual.ShouldBeNull();
        }

        [Fact]
        public async Task ResourceService_GetById_ReturnsResource()
        {
            var repoModel = new ResourceModel{Id = 123};
            _resourceRepository.Setup(r => r.GetById(It.IsAny<long>())).Returns(repoModel);
            var actual = await _resourceService.GetById(123);

            actual.ShouldBe(repoModel);

            _cacheManager.Verify(cm => cm.Set(It.Is<string>(k => k == string.Format(CacheKeyFormats.ResourceById, repoModel.Id))
                , It.IsAny<ResourceModel>(),
                It.IsAny<int>()), Times.Once);
        }
        #endregion

        #region Update

        [Fact]
        public async Task ResourceService_Update_ReturnErrorResponseOnNull()
        {

            var response1 = await _resourceService.UpdateAsync(null);
            response1.HasErrors().ShouldBeTrue();
            response1.Result.ShouldBe(ServiceResponseResult.Fail);

            var model = new ResourceModel();
            var response2 = await _resourceService.UpdateAsync(model);
            response2.HasErrors().ShouldBeTrue();
            response2.Result.ShouldBe(ServiceResponseResult.Fail);

        }

        [Fact]
        public async Task ResourceService_Update_Passes()
        {
            var model = new ResourceModel
            {
                Id = 123
            };

            var response = await _resourceService.UpdateAsync(model);
            response.HasErrors().ShouldBeFalse();
            response.Result.ShouldBe(ServiceResponseResult.Success);

            _auditHelper.Verify(a=>a.PrepareForUpdateAudity(It.Is<ResourceModel>(rm =>rm == model)), Times.Once);
            _resourceRepository.Verify(a => a.Update(It.Is<ResourceModel>(rm => rm == model)), Times.Once);
            _cacheManager.Verify(cm=>cm.Remove(It.Is<string>(k => k == string.Format(CacheKeyFormats.ResourceById, model.Id))), Times.Once);
        }
        #endregion
    }
}