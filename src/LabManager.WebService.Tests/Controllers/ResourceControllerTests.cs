#region Using

using System.Threading.Tasks;
using LabManager.WebService.Controllers;
using Shouldly;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using LabManager.Services.Resources;
using LabManager.Common.Domain.Resource;
using System.Collections.Generic;
using System;
using System.Linq;
using LabManager.WebService.Models.Resources;
using System.Threading;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using QAutomation.Core.Services;

#endregion

namespace LabManager.WebService.Tests.Controllers
{
    public class ResourceControllerTests
    {
        #region Read
        [Fact]
        public async Task ResourceController_GetAll_ReturnsEmptyCollection()
        {
            //Null Collection
            var resSrv = new Mock<IResourceService>();
            var ctrl = new ResourceController(resSrv.Object);

            //null as collection --> throws
            resSrv.Setup(s => s.GetAllAsync(It.IsAny<ResourceModel>())).ReturnsAsync(null as IEnumerable<ResourceModel>);
            Should.Throw<ArgumentNullException>(() => ctrl.GetAllAsync());

            //Empty Collection
            resSrv.Reset();
            resSrv.Setup(s => s.GetAllAsync(It.IsAny<ResourceModel>())).Returns(Task.FromResult(new ResourceModel[] { } as IEnumerable<ResourceModel>));
            var res = await ctrl.GetAllAsync();
            var content = Assert.IsType<OkObjectResult>(res);

            (content.Value as IEnumerable<ResourceApiModel>).Count().ShouldBe(0);
        }
        [Fact]
        public async Task ResourceController_GetAll_ReturnsNonEmptyCollection()
        {
            var expRes = new[]{
                new ResourceModel{FriendlyName="fn1"},
                new ResourceModel{FriendlyName="fn2"},
                new ResourceModel{FriendlyName="fn3"},
                new ResourceModel{FriendlyName="fn4"},
            };

            //Null Collection
            var resSrv = new Mock<IResourceService>();
            var ctrl = new ResourceController(resSrv.Object);

            resSrv.Setup(s => s.GetAllAsync(It.IsAny<ResourceModel>())).Returns(Task.FromResult(expRes as IEnumerable<ResourceModel>));
            var res = await ctrl.GetAllAsync();
            var content = Assert.IsType<OkObjectResult>(res);

            var array = (content.Value as IEnumerable<ResourceApiModel>);
            array.Count().ShouldBe(expRes.Count());
            var names = expRes.Select(e => e.FriendlyName);
            foreach (var i in array)
                names.ShouldContain(i.FriendlyName);
        }
        #endregion
        #region Create
        [Fact]
        public async Task ResourceController_Create_Fails()
        {
            var resSrv = new Mock<IResourceService>();
            var ctrl = new ResourceController(resSrv.Object);

            //null param
            var res0 = await ctrl.Create(null);
            Assert.IsType<BadRequestObjectResult>(res0);

            var apiModel = new ResourceApiModel { FriendlyName = "fn" };

            //Is was not incremented
            var srvRes = new ServiceResponse<ResourceModel>(new ResourceModel {FriendlyName = "fn"},
                ServiceRequestType.Create);
            srvRes.Result = ServiceResponseResult.Fail;

            resSrv.Setup(s => s.CreateAsync(It.IsAny<ResourceModel>()))
            .Returns(Task.FromResult(srvRes));

            var res1 = await ctrl.Create(apiModel);

            Assert.IsType<BadRequestObjectResult>(res1);

            //Id is null
            resSrv.Setup(s => s.CreateAsync(It.IsAny<ResourceModel>())).Returns(Task.FromResult(srvRes));
            var res2 = await ctrl.Create(apiModel);
            Assert.IsType<BadRequestObjectResult>(res2);
        }

        [Fact]
        public async Task ResourceController_Create_Pass()
        {
            var resSrv = new Mock<IResourceService>();
            var ctrl = new ResourceController(resSrv.Object);
            const int expId = 123;
            const string expName = "fn";

            var expResult = new ResourceModel { FriendlyName = expName, Id = expId };

            var serviceResponse = new ServiceResponse<ResourceModel>(expResult, ServiceRequestType.Create);
            serviceResponse.Result = ServiceResponseResult.Success;

            resSrv.Setup(s => s.CreateAsync(It.IsAny<ResourceModel>()))
            .Returns(Task.FromResult(serviceResponse));

            var apiModel = new ResourceApiModel { FriendlyName = expName };

            var res = await ctrl.Create(apiModel);
            Assert.IsType<CreatedResult>(res);
        }
        #endregion
        #region GetById

        [Fact]
        public async Task ResourceController_GetById_returnsNull()
        {
            var resSrv = new Mock<IResourceService>();
            resSrv.Setup(s => s.GetById(It.IsAny<long>())).Returns(Task.FromResult(null as ResourceModel));

            var ctrl = new ResourceController(resSrv.Object);
            (await ctrl.GetById(123)).ShouldBeOfType<BadRequestResult>();

        }

        [Fact]
        public async Task ResourceController_GetById_ReturnsResourceApiModel()
        {
            var resSrv = new Mock<IResourceService>();
            var expRes = new ResourceModel
            {
                FriendlyName = "fn",
                Id = 123
            };
            resSrv.Setup(s => s.GetById(It.IsAny<long>())).Returns(Task.FromResult(expRes));

            var ctrl = new ResourceController(resSrv.Object);
            var res = await ctrl.GetById(expRes.Id);
            var content = res.ShouldBeOfType<OkObjectResult>();
            var value = content.Value.ShouldBeOfType<ResourceApiModel>();
            value.FriendlyName.ShouldBe(expRes.FriendlyName);
            value.Id.ShouldBe(expRes.Id);

        }
        #endregion

        #region Update
        [Fact]
        public async Task ResourceController_Update_Fails()
        {
            var resSrv = new Mock<IResourceService>();
            var ctrl = new ResourceController(resSrv.Object);

            //null param
            var res0 = await ctrl.UpdateAsync(null);
            Assert.IsType<BadRequestObjectResult>(res0);

            var apiModel = new ResourceApiModel { FriendlyName = "fn" };

            //Is was not incremented
            var srvRes = new ServiceResponse<ResourceModel>(new ResourceModel { FriendlyName = "fn" },
                ServiceRequestType.Update);
            srvRes.Result = ServiceResponseResult.Fail;

            resSrv.Setup(s => s.UpdateAsync(It.IsAny<ResourceModel>()))
                .Returns(Task.FromResult(srvRes));

            var res1 = await ctrl.UpdateAsync(apiModel);

            Assert.IsType<BadRequestObjectResult>(res1);

            //Id is null
            resSrv.Setup(s => s.CreateAsync(It.IsAny<ResourceModel>())).Returns(Task.FromResult(srvRes));
            var res2 = await ctrl.UpdateAsync(apiModel);
            Assert.IsType<BadRequestObjectResult>(res2);
        }

        [Fact]
        public async Task ResourceController_Put_Pass()
        {
            var resSrv = new Mock<IResourceService>();
            var ctrl = new ResourceController(resSrv.Object);
            const int expId = 123;
            const string expName = "fn";

            var expResult = new ResourceModel { FriendlyName = expName, Id = expId };

            var serviceResponse = new ServiceResponse<ResourceModel>(expResult, ServiceRequestType.Update);
            serviceResponse.Result = ServiceResponseResult.Success;

            resSrv.Setup(s => s.UpdateAsync(It.IsAny<ResourceModel>()))
                .Returns(Task.FromResult(serviceResponse));

            var apiModel = new ResourceApiModel { FriendlyName = expName };

            var res = await ctrl.UpdateAsync(apiModel);
            Assert.IsType<OkObjectResult>(res);
        }
        #endregion
    }
}