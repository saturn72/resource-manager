using LabManager.Common.Domain.Command;
using LabManager.Services.Command;
using Saturn72.Core.Services;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LabManager.Services.Tests.Command
{
    public class CommandResourceServiceTests
    {
        [Theory]
        [MemberData(nameof(CommandResourceService_Command_OnInvalidModel_Data))]
        public async Task CommandResourceService_Command_OnInvalidModel(CommandModel cmdModel)
        {
            var cmdResourceService = new CommandResourceService();
            var res = await cmdResourceService.SendCommand(cmdModel);
            res.HasErrors().ShouldBeTrue();
            res.RequestType.ShouldBe(ServiceRequestType.Command);
            res.Result.ShouldBe(ServiceResponseResult.Fail);
            res.Model.ShouldBe(cmdModel);
        }

        public static IEnumerable<object[]> CommandResourceService_Command_OnInvalidModel_Data =>
            new[]
            {
                new object[] {null},
                new object[] {new CommandModel()},
                new object[] {new CommandModel {SessionId = "session"}},
                new object[] {new CommandModel {ResourceId = "resources"}},
            };
    }
}
