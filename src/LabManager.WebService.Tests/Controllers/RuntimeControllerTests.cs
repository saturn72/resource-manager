using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using LabManager.WebService.Controllers;
using LabManager.WebService.Models.Resources;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;
using LabManager.Services.Runtime;
using QAutomation.Core.Services;
using LabManager.Common.Domain.Runtime;
using LabManager.Common.Domain.Resource;
using Microsoft.VisualStudio.TestPlatform.Common;
using Xunit.Extensions;

namespace LabManager.WebService.Tests.Controllers
{
    public class RuntimeControllerTests
    {
        #region Get

        [Fact]
        public async Task RuntimeController_GetAsync_Fails_AllPermutations()
        {
            var possibleFaultyServiceResponses = new[]
            {
                //null response
                null as ServiceResponse<RuntimeSession>,
                //null model return
                new ServiceResponse<RuntimeSession>(null, ServiceRequestType.Read),
                //missing session Id
                new ServiceResponse<RuntimeSession>(new RuntimeSession(), ServiceRequestType.Read),
                //un-successful result
                new ServiceResponse<RuntimeSession>(new RuntimeSession(){SessionId = "123", Resource = new ResourceModel()}, ServiceRequestType.Read),
                //missing resource
                new ServiceResponse<RuntimeSession>(new RuntimeSession(){SessionId = "123", Resource = null}, ServiceRequestType.Read),

            };

            foreach (var sr in possibleFaultyServiceResponses)
                await ValidateRuntimeManagerGetAsyncFailures(sr);
        }

        public async Task ValidateRuntimeManagerGetAsyncFailures(ServiceResponse<RuntimeSession> srvRes)
        {
            var filter = new ResourceApiModel
            {
                FriendlyName = "123"
            };
            var rm = new Mock<IRuntimeManager>();
            rm.Setup(r => r.AssignResourceAsync(It.IsAny<ResourceModel>())).Returns(() => Task.FromResult(srvRes));
            var runtimeCtrl = new RuntimeController(rm.Object);

            var res1 = await runtimeCtrl.GetAsync(filter);
            var content1 = res1.ShouldBeOfType<NotFoundObjectResult>();
            (content1.Value as ResourceApiModel).ShouldBe(filter);
        }

        [Fact]
        public async Task ValidateRuntimeManagerGetAsyncReturnsResource()
        {
            var filter = new ResourceApiModel
            {
                FriendlyName = "123"
            };

            var expResource = new ResourceModel
            {
                Id = 222
            };

            var srvRes = new ServiceResponse<RuntimeSession>(
                    new RuntimeSession() {SessionId = "123", Resource = expResource}, ServiceRequestType.Read)
                {
                    Result = ServiceResponseResult.Success
                };
            var rm = new Mock<IRuntimeManager>();
            rm.Setup(r => r.AssignResourceAsync(It.IsAny<ResourceModel>())).Returns(() => Task.FromResult(srvRes));
            var runtimeCtrl = new RuntimeController(rm.Object);

            var res1 = await runtimeCtrl.GetAsync(filter);
            var content1 = res1.ShouldBeOfType<AcceptedResult>();
            var actualApiModel = content1.Value as ResourceApiModel;
            actualApiModel.ShouldNotBeNull();
            actualApiModel.Id.ShouldBe(expResource.Id);
        }
        #endregion
    }
}