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
            var startResponseData = new StartResponseData {Started = false};
            var srvRes =
                new ServiceResponse<StartResponseData>(startResponseData,
                    ServiceRequestType.Launch);
            iSrv.Setup(i => i.Start(It.IsAny<long>())).Returns(()=> Task.FromResult(srvRes));
            var ic = new InstanceController(iSrv.Object);
            var res1 = await ic.StartAsync(resourceId: 123);
            res1.ShouldBeOfType<BadRequestObjectResult>();

            startResponseData.Started = true;
            var res2 = await ic.StartAsync(resourceId: 123);
            res2.ShouldBeOfType<OkResult>();
        }
    }
}
