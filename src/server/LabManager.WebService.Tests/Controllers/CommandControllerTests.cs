using System.Collections.Generic;
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
        [Theory]
        [MemberData(nameof(CommandController_SendCommand_ReturndBadRequest_Data))]
        public async Task CommandController_SendCommand_ReturndBadRequest(CommandApiModel apiModel)
        {
            var ctrl = new CommandController(null);

            var res = await ctrl.SendCommand(apiModel);
            res.ShouldBeOfType<BadRequestObjectResult>();
        }

        public static IEnumerable<object[]> CommandController_SendCommand_ReturndBadRequest_Data =>
            new[]
            {
                new object[] {null},
                new object[] {new CommandApiModel(),},
                new object[] {new CommandApiModel {ResourceId = "123"},},
                new object[] {new CommandApiModel {SessionId = "123"},},
            };

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

            var commandApiModel = new CommandApiModel{ResourceId = "some-resource", SessionId = "some-session"};
            var res = await ctrl.SendCommand(commandApiModel);
            res.ShouldBeOfType<OkObjectResult>();
        }
    }
}
