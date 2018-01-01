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
using Saturn72.Core.Caching;
using Saturn72.Core.Services;
using Saturn72.Core;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Audit;

namespace LabManager.Services.Tests.Instance
{
    public class InstanceServiceTests
    {
        #region Start

        [Fact]
        public async Task InstancService_Start_IllegalResourceId()
        {
            var iSrv = new InstanceService(null, null, null, null, null);
            var res1 = await iSrv.StartAsync(0);
            res1.Model.Executed.ShouldBeFalse();
            res1.Result.ShouldBe(ServiceResponseResult.Fail);
            var res2 = await iSrv.StartAsync(-100);
            res2.Model.Executed.ShouldBeFalse();
            res2.Result.ShouldBe(ServiceResponseResult.Fail);
        }

        [Fact]
        public async Task InstancService_Start_ResourceNotExistsOrUnavilalbe()
        {
            ResourceModel rm = null;
            var rr = new Mock<IResourceRepository>();
            rr.Setup(r => r.GetById(It.IsAny<long>())).Returns(() => rm);

            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<ResourceModel>(It.IsAny<string>())).Returns(null as ResourceModel);
            var iSrv = new InstanceService(rr.Object, cm.Object, null, null, null);
            var res1 = await iSrv.StartAsync(1111);
            res1.Model.Executed.ShouldBeFalse();
            res1.Result.ShouldBe(ServiceResponseResult.Fail);

            //returns unavailble
            rm = new ResourceModel
            {
                Status = ResourceStatus.Unavailable
            };
            var res2 = await iSrv.StartAsync(1111);
            res2.Model.Executed.ShouldBeFalse();
            res2.Result.ShouldBe(ServiceResponseResult.Fail);
        }

        [Fact]
        public async Task InstancService_Start_StartsResource()
        {
            var callList = new List<string>();
            var wc = new Mock<IWorkContext>();
            wc.Setup(w => w.CurrentUserId).Returns(123);
            var ah = new Mock<AuditHelper>(wc.Object);
            ah.Setup(a => a.PrepareForUpdateAudity(It.IsAny<IUpdateAudit>()))
                .Callback<IUpdateAudit>(r => callList.Add("ah_" + (r as ResourceModel).Status.ToString()));

            var ep = new Mock<IEventPublisher>();
            ep.Setup(a => a.PublishAsync(It.IsAny<DomainModelUpdatedEvent<ResourceModel>>(), CancellationToken.None))
                .Callback<DomainModelUpdatedEvent<ResourceModel>, CancellationToken>((r,c) => callList.Add("ep_" + r.Model.Status.ToString()));
            var rr = new Mock<IResourceRepository>();
            rr.Setup(r => r.Update(It.IsAny<ResourceModel>()))
                .Callback<ResourceModel>(r => callList.Add("rr_" + r.Status.ToString()));

            var rm = new ResourceModel
            {
                Id = 123,
                SshUsername = "some-username",
                SshPassword = "some-password",
                IpAddress = "192.168.2.3"
            };
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<ResourceModel>(It.IsAny<string>())).Returns(rm);
            var rc = new Mock<IResourceController>();

            //Returns 0 (started) from resource commander
            rc.Setup(r => r.Start(It.IsAny<ResourceModel>())).Returns(0);

            var iSrv = new InstanceService(rr.Object, cm.Object, ah.Object, ep.Object, rc.Object);
            var res1 = await iSrv.StartAsync(11);
            res1.RequestType.ShouldBe(ServiceRequestType.Command);
            res1.Result.ShouldBe(ServiceResponseResult.Success);

            res1.Model.Executed.ShouldBeTrue();
            rc.Verify(r => r.Start(It.IsAny<ResourceModel>()), Times.Once);
            //verify
            foreach (var item in new[] { "ah", "rr", "ep" })
            {
                callList.Count(c => c.StartsWith(item + "_")).ShouldBe(2);
                callList.Any(c => c == item + "_" + ResourceStatus.Started.ToString()).ShouldBeTrue();
                callList.Any(c => c == item + "_" + ResourceStatus.Available.ToString()).ShouldBeTrue();
            }

            //Returns -1 (NOT-STARTED) from resource commander
            rc.ResetCalls();
            rc.Setup(r => r.Start(It.IsAny<ResourceModel>())).Returns(-1);
            callList.Clear();

            iSrv = new InstanceService(rr.Object, cm.Object, ah.Object, ep.Object, rc.Object);
            var res2 = await iSrv.StartAsync(11);
            res2.RequestType.ShouldBe(ServiceRequestType.Command);
            res2.Result.ShouldBe(ServiceResponseResult.Fail);
            res2.Model.Executed.ShouldBeFalse();

            //verify
            rc.Verify(r => r.Start(It.IsAny<ResourceModel>()), Times.Once);
            foreach (var item in new[] { "ah", "rr", "ep" })
            {
                callList.Count(c => c.StartsWith(item + "_")).ShouldBe(2);
                callList.Any(c => c == item + "_" + ResourceStatus.Started.ToString()).ShouldBeTrue();
                callList.Any(c => c == item + "_" + ResourceStatus.Available.ToString()).ShouldBeTrue();
            }
        }

        #endregion

        #region Stop
        [Fact]
        public async Task InstancService_Stop_IllegalResourceId()
        {
            var iSrv = new InstanceService(null, null, null, null, null);
            var res1 = await iSrv.StopAsync(0);
            res1.Model.Executed.ShouldBeFalse();
            res1.Result.ShouldBe(ServiceResponseResult.Fail);
            var res2 = await iSrv.StopAsync(-100);
            res2.Model.Executed.ShouldBeFalse();
            res2.Result.ShouldBe(ServiceResponseResult.Fail);
        }

        [Fact]
        public async Task InstancService_Stop_ResourceNotExistsOrUnavilalbe()
        {
            ResourceModel rm = null;
            var rr = new Mock<IResourceRepository>();
            rr.Setup(r => r.GetById(It.IsAny<long>())).Returns(() => rm);

            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<ResourceModel>(It.IsAny<string>())).Returns(null as ResourceModel);
            var iSrv = new InstanceService(rr.Object, cm.Object, null, null, null);
            var res1 = await iSrv.StopAsync(1111);
            res1.Model.Executed.ShouldBeFalse();
            res1.Result.ShouldBe(ServiceResponseResult.Fail);

            //returns unavailble
            rm = new ResourceModel
            {
                Status = ResourceStatus.Unavailable
            };
            var res2 = await iSrv.StopAsync(1111);
            res2.Model.Executed.ShouldBeFalse();
            res2.Result.ShouldBe(ServiceResponseResult.Fail);
        }

        [Fact]
        public async Task InstancService_Stop_StopsResource()
        {
            var callList = new List<string>();
            var wc = new Mock<IWorkContext>();
            wc.Setup(w => w.CurrentUserId).Returns(123);
            var ah = new Mock<AuditHelper>(wc.Object);
            ah.Setup(a => a.PrepareForUpdateAudity(It.IsAny<IUpdateAudit>()))
                .Callback<IUpdateAudit>(r => callList.Add("ah_" + (r as ResourceModel).Status.ToString()));

            var ep = new Mock<IEventPublisher>();
            ep.Setup(a => a.PublishAsync(It.IsAny<DomainModelUpdatedEvent<ResourceModel>>(), CancellationToken.None))
                .Callback<DomainModelUpdatedEvent<ResourceModel>, CancellationToken>((e,c) => callList.Add("ep_" + e.Model.Status.ToString()));

            var rr = new Mock<IResourceRepository>();
            rr.Setup(r => r.Update(It.IsAny<ResourceModel>()))
                .Callback<ResourceModel>(r => callList.Add("rr_" + r.Status.ToString()));

            var rm = new ResourceModel
            {
                Id = 123,
                SshUsername = "some-username",
                SshPassword = "some-password",
                IpAddress = "192.168.2.3"
            };
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Get<ResourceModel>(It.IsAny<string>())).Returns(rm);
            var rc = new Mock<IResourceController>();

            //Returns 0 (Stopped) from resource commander
            rc.Setup(r => r.Stop(It.IsAny<ResourceModel>())).Returns(0);

            var iSrv = new InstanceService(rr.Object, cm.Object, ah.Object, ep.Object, rc.Object);
            var res1 = await iSrv.StopAsync(11);
            res1.RequestType.ShouldBe(ServiceRequestType.Command);
            res1.Result.ShouldBe(ServiceResponseResult.Success);

            res1.Model.Executed.ShouldBeTrue();

            rc.Verify(r => r.Stop(It.IsAny<ResourceModel>()), Times.Once);
            //verify
            foreach (var item in new[] { "ah", "rr", "ep" })
            {
                callList.Count(c => c.StartsWith(item + "_")).ShouldBe(2);
                callList.Any(c => c == item + "_" + ResourceStatus.Stopped.ToString()).ShouldBeTrue();
                callList.Any(c => c == item + "_" + ResourceStatus.Available.ToString()).ShouldBeTrue();
            }

            //Returns -1 (NOT-Stopped) from resource commander
            rc.ResetCalls();
            rc.Setup(r => r.Stop(It.IsAny<ResourceModel>())).Returns(-1);
            callList.Clear();

            iSrv = new InstanceService(rr.Object, cm.Object, ah.Object, ep.Object, rc.Object);
            var res2 = await iSrv.StopAsync(11);
            res2.RequestType.ShouldBe(ServiceRequestType.Command);
            res2.Result.ShouldBe(ServiceResponseResult.Fail);
            res2.Model.Executed.ShouldBeFalse();
            rc.Verify(r => r.Stop(It.IsAny<ResourceModel>()), Times.Once);
            //verify
            foreach (var item in new[] { "ah", "rr", "ep" })
            {
                callList.Count(c => c.StartsWith(item + "_")).ShouldBe(2);
                callList.Any(c => c == item + "_" + ResourceStatus.Stopped.ToString()).ShouldBeTrue();
                callList.Any(c => c == item + "_" + ResourceStatus.Available.ToString()).ShouldBeTrue();
            }
        }

        #endregion
    }
}
