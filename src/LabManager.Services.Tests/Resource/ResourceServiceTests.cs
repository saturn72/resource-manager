using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using QAutomation.Core.Services.Events;
using LabManager.Services.Resources;
using LabManager.Common.Domain.Resource;
using System.Linq;
using Shouldly;
using QAutomation.Core.Services;

namespace LabManager.Services.Tests.Resource
{
    public class ResourceServiceTests
    {
        #region GetAll
        [Fact]
        public async Task ResourceService_GetAllAsync_Fails()
        {
            var resRepo = new Mock<IResourceRepository>();
            var srv = new ResourceService(resRepo.Object, null);

            //return null
            resRepo.Setup(r => r.GetAll()).Returns(null as IEnumerable<ResourceModel>);
            (await srv.GetAllAsync()).Count().ShouldBe(0);

            //returns empty collection
            resRepo.Setup(r => r.GetAll()).Returns(new ResourceModel[] { } as IEnumerable<ResourceModel>);
            (await srv.GetAllAsync()).Count().ShouldBe(0);
        }

        [Fact]
        public async Task ResourceService_GetAllAsync_Passes()
        {
            var expRes = new ResourceModel[]{
                new ResourceModel{FriendlyName = "fn1"},
                new ResourceModel{FriendlyName = "fn2"},
                new ResourceModel{FriendlyName = "fn3"},
                new ResourceModel{FriendlyName = "fn4"},
            };
            var resRepo = new Mock<IResourceRepository>();
            var srv = new ResourceService(resRepo.Object, null);

            resRepo.Setup(r => r.GetAll()).Returns(expRes as IEnumerable<ResourceModel>);
            var col = (await srv.GetAllAsync());
            col.Count().ShouldBe(expRes.Length);
            var names = expRes.Select(s => s.FriendlyName);

            foreach (var c in col)
                names.ShouldContain(c.FriendlyName);
        }

        [Fact]
        public async Task ResourceService_Add_Fails()
        {
            var resRepo = new Mock<IResourceRepository>();
            var srv = new ResourceService(resRepo.Object, null);

            ResourceModel toCreate = null;
            var actual = await srv.CreateAsync(toCreate);

            actual.Model.ShouldBeNull();
            actual.HasErrors().ShouldBeTrue();
            actual.Result.ShouldBe(ServiceResponseResult.Fail);
            actual.RequestType.ShouldBe(ServiceRequestType.Create);

            toCreate = new ResourceModel { };
            actual = await srv.CreateAsync(toCreate);

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
                FriendlyName = fName ,
                IpAddress = "192.168.2.2"
            };
            var createdId = (long)123;

            var resRepo = new Mock<IResourceRepository>();
            var ep = new Mock<IEventPublisher>();
            var srv = new ResourceService(resRepo.Object, ep.Object);

            resRepo.Setup(r => r.Create(It.IsAny<ResourceModel>())).Returns(createdId);
            resRepo.Setup(r => r.GetById(It.IsAny<long>())).Returns(() =>
            {
                toCreate.Id = createdId;
                return toCreate;
            });
            var actual = await srv.CreateAsync(toCreate);

            actual.Result.ShouldBe(ServiceResponseResult.Success);
            actual.Model.FriendlyName.ShouldBe(fName);
            actual.Model.Id.ShouldBe(createdId);

            //verify event was raised
            ep.Verify(e => e.PublishAsync(It.IsAny<DomainModelCreatedEvent<ResourceModel>>()), Times.Once);
            ep.Verify(e => e.Publish(It.IsAny<DomainModelCreatedEvent<ResourceModel>>()), Times.Once);
        }
        #endregion

        #region GetById

        [Fact]
        public async Task ResourceService_GetById_ReturnsNull()
        {
            var resRepo = new Mock<IResourceRepository>();
            resRepo.Setup(r => r.GetById(It.IsAny<long>())).Returns(null as ResourceModel);
            var srv = new ResourceService(resRepo.Object, null);
            var actual = await srv.GetById(123);

            actual.ShouldBeNull();
        }
        #endregion
    }
}