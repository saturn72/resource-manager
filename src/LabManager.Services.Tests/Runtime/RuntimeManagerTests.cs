using System;
using System.Collections.Generic;
using LabManager.Common.Domain.Resource;
using LabManager.Services.Runtime;
using QAutomation.Core.Services;
using Shouldly;
using System.Threading.Tasks;
using Moq;
using Xunit;
using LabManager.Services.Resources;

namespace LabManager.Services.Tests.Runtime
{
    public class RuntimeManagerTests
    {
        [Fact]
        public async Task RuntimeManager_AssignResourceAsync_DoesNotFetchItems()
        {
            var filter = new ResourceModel
            {
                Id = 123
            };

            var rsSrv = new Mock<IResourceService>();
            rsSrv.Setup(r => r.GetAllAsync(It.IsAny<ResourceModel>()))
                .Returns(Task.FromResult(null as IEnumerable<ResourceModel>));

            var mgr = new RuntimeManager(rsSrv.Object);
            var res = await mgr.AssignResourceAsync(filter);
            res.RequestType = ServiceRequestType.Read;
            res.Result.ShouldBe(ServiceResponseResult.Fail);

            //does not get items
            //
            //gets item
        }
    }
}
