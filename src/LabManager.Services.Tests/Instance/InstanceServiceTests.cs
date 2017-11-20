using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LabManager.Services.Instance;
using Shouldly;
using System.Threading.Tasks;
using Moq;
using Xunit;
using LabManager.Services.Resources;
using LabManager.Common.Domain.Resource;
using QAutomation.Core.Services.Caching;
using QAutomation.Core.Services;
using QAutomation.Core;
using QAutomation.Core.Services.Events;

namespace LabManager.Services.Tests.Instance
{
    public class InstanceServiceTests
    {
        [Fact]
        public async Task InstancService_Start_IllegalResourceId()
        {
            var iSrv = new InstanceService(null, null, null, null);
            var res1 = await iSrv.Start(0);
            res1.ShouldBeFalse();

            var res2 = await iSrv.Start(0);
            res2.ShouldBeFalse();
        }

        [Fact]
        public async Task InstancService_Start_ResourceNotExists()
        {
            ResourceModel rm = null;
            var rr = new Mock<IResourceRepository>();
            rr.Setup(r => r.GetById(It.IsAny<long>())).Returns(() => rm);

            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<ResourceModel>(It.IsAny<string>())).Returns(null as ResourceModel);
            var iSrv = new InstanceService(rr.Object, cm.Object, null, null);
            var res1 = await iSrv.Start(1111);
            res1.ShouldBeFalse();

            //returns unavailble
            rm = new ResourceModel
            {
                Status = ResourceStatus.Unavailable
            };
            var res2 = await iSrv.Start(1111);
            res2.ShouldBeFalse();
        }

        [Fact]
        public async Task InstancService_Start_StartsResource()
        {
            var callList = new List<string>();
            var wc = new Mock<IWorkContext>();
            wc.Setup(w => w.CurrentUserId).Returns(123);
            var ah = new Mock<AuditHelper>(wc.Object);
            ah.Setup(a => a.PrepareForUpdateAudity(It.IsAny<ResourceModel>()))
                .Callback<ResourceModel>(r => callList.Add("ah_" + r.Status.ToString()));

            var ep = new Mock<IEventPublisher>();
            ep.Setup(a => a.PublishAsync(It.IsAny< DomainModelUpdatedEvent<ResourceModel>>()))
                .Callback< DomainModelUpdatedEvent<ResourceModel>>(r => callList.Add("ep_" + r.Model.Status.ToString()));
            var rr = new Mock<IResourceRepository>();
            rr.Setup(r=>r.Update(It.IsAny<ResourceModel>()))
                .Callback<ResourceModel>(r => callList.Add("rr_" + r.Status.ToString()));

            var rm = new ResourceModel
            {
                Id = 123
            };
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<ResourceModel>(It.IsAny<string>())).Returns(rm);
            var iSrv = new InstanceService(rr.Object, cm.Object, ah.Object, ep.Object);
            var res1 = await iSrv.Start(11);
            res1.ShouldBeTrue();

            //verify
            foreach (var item in new[] {"ah", "rr", "ep"})
            {
                callList.Count(c => c.StartsWith(item + "_")).ShouldBe(2);
                callList.Any(c => c == item + "_" + ResourceStatus.Started.ToString()).ShouldBeTrue();
                callList.Any(c => c == item + "_" + ResourceStatus.Available.ToString()).ShouldBeTrue();
            }
        }
    }
}
