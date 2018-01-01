using System.Threading.Tasks;
using LabManager.WebService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;
using LabManager.Services.Command;
using LabManager.WebService.Models.Command;
using LabManager.Common.Domain.Command;
using Saturn72.Core.Services;

namespace LabManager.WebService.Tests.Controllers
{
    public class CommandControllerTests
    {
        [Fact]
        public async Task CommandController_SendCommand_ReturndBadRequest()
        {
            var ctrl = new CommandController(null);

            var res = await ctrl.SendCommand(null);
            res.ShouldBeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task CommandController_SendCommand_ReturndErrorFromService()
        {
            var srvRes = new ServiceResponse<CommandModel>(ServiceRequestType.Command)
            {
                ErrorMessage = "dadada"
            };
            var srv = new Mock<ICommandResourceService>();
            srv.Setup(s => s.SendCommand(It.IsAny<CommandModel>())).Returns(Task.FromResult(srvRes));

            var ctrl = new CommandController(srv.Object);

            var res = await ctrl.SendCommand(new CommandApiModel());
            res.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CommandController_SendCommand_ReturndSuccessFromService()
        {
            var srvRes = new ServiceResponse<CommandModel>(ServiceRequestType.Command)
            {
                Result = ServiceResponseResult.Success
            };

            var srv = new Mock<ICommandResourceService>();
            srv.Setup(s => s.SendCommand(It.IsAny<CommandModel>())).Returns(Task.FromResult(srvRes));

            var ctrl = new CommandController(srv.Object);

            var res = await ctrl.SendCommand(new CommandApiModel());
            res.ShouldBeOfType<OkObjectResult>();
        }
    }
}
