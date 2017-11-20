using System.Threading.Tasks;
using LabManager.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Shouldly;
using LabManager.Services.Instance;
using QAutomation.Core.Services;

namespace LabManager.WebService.Tests.Controllers
{
    public class InstanceControllerTests
    {

        #region Start

        [Fact]
        public async Task InstanceController_StartInstance_IllegalResourceId()
        {
            var ic = new InstanceController(null);

            var res1 = await ic.StartAsync(0);
            res1.ShouldBeOfType<BadRequestObjectResult>();

            var res2 = await ic.StartAsync(-123);
            res2.ShouldBeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public async Task InstanceController_StartInstance()
        {
            var iSrv = new Mock<IInstanceService>();
            var startResponseData = new ResourceExecutionResponseData();
            var srvRes =
                new ServiceResponse<ResourceExecutionResponseData>(startResponseData,
                    ServiceRequestType.Command);
            iSrv.Setup(i => i.StartAsync(It.IsAny<long>())).Returns(()=> Task.FromResult(srvRes));
            var ic = new InstanceController(iSrv.Object);
            var res1 = await ic.StartAsync(resourceId: 123);
            res1.ShouldBeOfType<BadRequestObjectResult>();

            startResponseData.Executed = true;
            var res2 = await ic.StartAsync(resourceId: 123);
            res2.ShouldBeOfType<OkResult>();
        }

        #endregion

        #region Stop

        [Fact]
        public async Task InstanceController_StopInstance_IllegalResourceId()
        {
            var ic = new InstanceController(null);

            var res1 = await ic.StopAsync(0);
            res1.ShouldBeOfType<BadRequestObjectResult>();

            var res2 = await ic.StopAsync(-123);
            res2.ShouldBeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public async Task InstanceController_StopInstance()
        {
            var iSrv = new Mock<IInstanceService>();
            var stopResponseData = new ResourceExecutionResponseData();
            var srvRes = new ServiceResponse<ResourceExecutionResponseData>(stopResponseData,
                    ServiceRequestType.Command);
            iSrv.Setup(i => i.StopAsync(It.IsAny<long>())).Returns(() => Task.FromResult(srvRes));
            var ic = new InstanceController(iSrv.Object);
            var res1 = await ic.StopAsync(resourceId: 123);
            res1.ShouldBeOfType<BadRequestObjectResult>();

            stopResponseData.Executed = true;
            var res2 = await ic.StopAsync(resourceId: 123);
            res2.ShouldBeOfType<OkResult>();
        }

        #endregion
    }
}
