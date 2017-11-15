using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using QAutomation.Core.Services;
using LabManager.Services.Runtime;
using LabManager.Services.Resources;
using LabManager.Common.Domain.Resource;
using Shouldly;
using QAutomation.Core.Services.Caching;

namespace LabManager.Services.Tests.Runtime
{
    public class RuntimeManagerTests
    {
        #region AssignResourceAsync

        [Fact]
        public async Task RuntimeManager_AssignResourceAsync_EmptySession()
        {
            var mgr = new RuntimeManager(null, null);
            foreach (var sessionId in new[] { null, string.Empty, "", "   " })
            {
                var srvRes = await mgr.AssignResourcesAsync(sessionId);
                srvRes.HasErrors().ShouldBeTrue();
                srvRes.Result.ShouldBe(ServiceResponseResult.Fail);
            }
        }

        [Fact]
        public async Task RuntimeManager_AssignResourceAsync_SessionExpired()
        {
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<ResourceAssignmentResponse>(It.IsAny<string>()))
                .Returns(null as ResourceAssignmentResponse);

            var mgr = new RuntimeManager(null, cm.Object);
            var srvRes = await mgr.AssignResourcesAsync("some-session-id");
            srvRes.HasErrors().ShouldBeTrue();
            srvRes.Result.ShouldBe(ServiceResponseResult.Fail);
        }

        [Fact]
        public async Task RuntimeManager_AssignResourceAsync_Assignes()
        {
            var rr = new Mock<IResourceService>();

            var cm = new Mock<ICacheManager>();
            var someSessionId = "some-session-id";
            var resources = new[]
            {
                new ResourceModel {Id = 1},
                new ResourceModel {Id = 2},
                new ResourceModel {Id = 3},
                new ResourceModel {Id = 4}
            };

            cm.Setup(c => c.Get<ResourceAssignmentResponse>(It.IsAny<string>()))
                .Returns(new ResourceAssignmentResponse(someSessionId, null)
                {
                    Resources = resources
                });

            var mgr = new RuntimeManager(rr.Object, cm.Object);
            var srvRes = await mgr.AssignResourcesAsync(someSessionId);
            srvRes.HasErrors().ShouldBeFalse();
            srvRes.Result.ShouldBe(ServiceResponseResult.Success);
            srvRes.Model.Status.ShouldBe(ResourceAssignmentStatus.Assigned);

            foreach (var r in resources)
            {
                r.Status.ShouldBe(ResourceStatus.Assigned);
                rr.Verify(rs=>rs.UpdateAsync(It.Is<ResourceModel>(x=>x == r)), Times.Once);
            }
        }

        #endregion

        #region RequestResourceAssignmentAsync
        [Fact]
        public async Task RuntimeManager_RequestResourceAssignmentAsync_DoesNotFetchEnoughItems()
        {
            var assReq = new ResourceAssignmentRequest
            {
                RequiredResources = new[]
                {
                    new ResourceModel
                    {
                        Id = 123
                    },
                    new ResourceModel
                    {
                        Id = 234
                    }
                }
            };

            var rsSrv = new Mock<IResourceService>();
            rsSrv.Setup(r => r.GetAllAsync(It.IsAny<ResourceModel>()))
                .Returns(Task.FromResult(new[] { new ResourceModel { Active = true } } as IEnumerable<ResourceModel>));

            var mgr = new RuntimeManager(rsSrv.Object, null);
            var res = await mgr.RequestResourceAssignmentAsync(assReq);
            res.RequestType = ServiceRequestType.Read;
            res.Result.ShouldBe(ServiceResponseResult.Fail);
            res.HasErrors().ShouldBeTrue();
        }

        [Fact]
        public async Task RuntimeManager_RequestResourceAssignmentAsync_DoesNotFetchItems()
        {
            var assReq = new ResourceAssignmentRequest
            {
                RequiredResources = new[]
                {
                    new ResourceModel
                    {
                        Id = 123
                    }
                }
            };

            var rsSrv = new Mock<IResourceService>();
            rsSrv.Setup(r => r.GetAllAsync(It.IsAny<ResourceModel>()))
                .Returns(Task.FromResult(null as IEnumerable<ResourceModel>));

            var mgr = new RuntimeManager(rsSrv.Object, null);
            var res = await mgr.RequestResourceAssignmentAsync(assReq);
            res.RequestType = ServiceRequestType.Read;
            res.Result.ShouldBe(ServiceResponseResult.Fail);
        }

        [Fact]
        public async Task RuntimeManager_RequestResourceAssignmentAsync_Passes()
        {
            var assReq = new ResourceAssignmentRequest
            {
                RequiredResources = new[]
                {
                    new ResourceModel
                    {
                        Id = 123
                    }
                }
            };
            var cm = new Mock<ICacheManager>();
            var rsSrv = new Mock<IResourceService>();
            var resourceCollection = new[]
            {
                new ResourceModel {Active = true},
                new ResourceModel {Active = true}
            };
            rsSrv.Setup(r => r.GetAllAsync(It.IsAny<ResourceModel>()))
                .Returns(Task.FromResult(resourceCollection as IEnumerable<ResourceModel>));

            var mgr = new RuntimeManager(rsSrv.Object, cm.Object);
            var res = await mgr.RequestResourceAssignmentAsync(assReq);
            res.RequestType = ServiceRequestType.Read;
            res.Result.ShouldBe(ServiceResponseResult.Success);
            res.HasErrors().ShouldBeFalse();

            //cached session
            cm.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<ResourceAssignmentResponse>(), It.IsAny<int>()),
                Times.Once);
        }

        #endregion
    }
}