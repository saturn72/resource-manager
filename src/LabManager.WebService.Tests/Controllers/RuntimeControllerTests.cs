using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabManager.WebService.Controllers;
using LabManager.WebService.Models.Resources;
using LabManager.WebService.Models.Runtime;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;
using LabManager.Services.Runtime;
using QAutomation.Core.Services;
using LabManager.Common.Domain.Resource;

namespace LabManager.WebService.Tests.Controllers
{
    public class RuntimeControllerTests
    {

        #region Get

        public async Task ValidateRuntimeManagerGetAsyncFailures(ServiceResponse<ResourceAssignmentResponse> srvRes)
        {
            var assReq = new ResourceAssignmentRequestApiModel
            {
                RequiredResources = new[]
                {
                    new ResourceApiModel
                    {
                        FriendlyName = "123"
                    }
                }
            };
            var rm = new Mock<IRuntimeManager>();
            rm.Setup(r => r.RequestResourceAssignmentAsync(It.IsAny<ResourceAssignmentRequest>(), It.IsAny<bool>()))
                .Returns(() => Task.FromResult(srvRes));
            var runtimeCtrl = new RuntimeController(rm.Object);

            var res1 = await runtimeCtrl.RequestAssignment(assReq);
            var content1 = res1.ShouldBeOfType<NotFoundObjectResult>();
            (content1.Value as ResourceAssignmentRequestApiModel).ShouldBe(assReq);
        }

        [Fact]
        public async Task RuntimeController_GetAsync_Fails_AllPermutations()
        {
            var sessionId = "session-id";
            var runtimeSession = new ResourceAssignmentResponse(sessionId, null);
            var possibleFaultyServiceResponses = new[]
            {
                //null response
                null as ServiceResponse<ResourceAssignmentResponse>,
                //null model return
                new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Read),
                //missing session Id
                new ServiceResponse<ResourceAssignmentResponse>(runtimeSession, ServiceRequestType.Read),
                //un-successful result
                new ServiceResponse<ResourceAssignmentResponse>(runtimeSession, ServiceRequestType.Read)
                {
                    Model = new ResourceAssignmentResponse(sessionId, null) {Resources = new[] {new ResourceModel()}}
                },
                //missing resource
                new ServiceResponse<ResourceAssignmentResponse>(runtimeSession, ServiceRequestType.Read)
                {
                    Model = runtimeSession
                },
                //missing resource
                new ServiceResponse<ResourceAssignmentResponse>(runtimeSession, ServiceRequestType.Read)
                {
                    Model = runtimeSession,
                    ErrorMessage = "some-error"
                }
            };

            foreach (var sr in possibleFaultyServiceResponses)
                await ValidateRuntimeManagerGetAsyncFailures(sr);
        }

        [Fact]
        public async Task ValidateRuntimeManagerGetAsyncReturnsResource()
        {
            var filter = new ResourceAssignmentRequestApiModel
            {
                RequiredResources = new[]
                {
                    new ResourceApiModel
                    {
                        FriendlyName = "123"
                    }
                }
            };

            var expResource = new[]
            {
                new ResourceModel
                {
                    Id = 222
                }
            };

            var srvRes = new ServiceResponse<ResourceAssignmentResponse>(
                new ResourceAssignmentResponse("123", null) {Resources = expResource}, ServiceRequestType.Read)
            {
                Result = ServiceResponseResult.Success
            };
            var rm = new Mock<IRuntimeManager>();
            rm.Setup(r => r.RequestResourceAssignmentAsync(It.IsAny<ResourceAssignmentRequest>(), It.IsAny<bool>()))
                .Returns(() => Task.FromResult(srvRes));
            var runtimeCtrl = new RuntimeController(rm.Object);

            var res1 = await runtimeCtrl.RequestAssignment(filter);
            var content1 = res1.ShouldBeOfType<OkObjectResult>();
            var jo = TestUtil.ExtractJObject(content1.Value);
            var actualResources = TestUtil.ExtractJArray(jo["resources"]);
            actualResources.ShouldNotBeNull();
            actualResources.Count().ShouldBe(expResource.Count());
        }

        #endregion

        #region Post

        [Fact]
        public async Task RuntimeController_Post_ReturnNullFromService()
        {
            var responses = new[]
            {
                (null as ServiceResponse<ResourceAssignmentResponse>),
                //has error messages
                new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Create)
                {
                    ErrorMessage = "some-error-message"
                },
                //Result != success
                new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Fail
                },
                //model is null
                new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Success
                },
                //session Id is null
                new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Success,
                    Model = new ResourceAssignmentResponse(null, null)
                },
                //Resourcesw are null
                new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Success,
                    Model = new ResourceAssignmentResponse("123", null)
                },
                //Status!=Assigned
                new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Create)
                {
                    Result = ServiceResponseResult.Success,
                    Model = new ResourceAssignmentResponse("123", null)
                    {
                        Resources = new[]
                        {
                            new ResourceModel()
                        }
                    }
                },
            };
            foreach (var sr in responses)
                await ValidateRuntimeManagerPostAsyncFailures(sr);
        }

        private async Task ValidateRuntimeManagerPostAsyncFailures(
            ServiceResponse<ResourceAssignmentResponse> serviceResponse)
        {
            var sessionId = "session-id";
            var rm = new Mock<IRuntimeManager>();
            rm.Setup(r => r.AssignResourcesAsync(It.IsAny<string>())).Returns(() => Task.FromResult(serviceResponse));

            var ctrl = new RuntimeController(rm.Object);
            var res = await ctrl.AssignSessionAsync(sessionId);
            var content = res.ShouldBeOfType<BadRequestObjectResult>();
            var jo = TestUtil.ExtractJObject(content.Value);
            ((string) jo["sessionId"]).ShouldBe(sessionId);
        }

        [Fact]
        public async Task RuntimeController_Post_AssignApproved()
        {
            var srvRes = new ServiceResponse<ResourceAssignmentResponse>(null, ServiceRequestType.Create)
            {
                Result = ServiceResponseResult.Success,
                Model = new ResourceAssignmentResponse("123", null)
                {
                    Resources = new[]
                    {
                        new ResourceModel()
                    },
                    Status = ResourceAssignmentStatus.Assigned
                }

            };

            var sessionId = "session-id";
            var rm = new Mock<IRuntimeManager>();
            rm.Setup(r => r.AssignResourcesAsync(It.IsAny<string>())).Returns(Task.FromResult(srvRes));

            var ctrl = new RuntimeController(rm.Object);
            var res = await ctrl.AssignSessionAsync(sessionId);
            var content = res.ShouldBeOfType<OkObjectResult>();
            content.Value.ShouldBe(sessionId);
        }

        #endregion
    }
}
