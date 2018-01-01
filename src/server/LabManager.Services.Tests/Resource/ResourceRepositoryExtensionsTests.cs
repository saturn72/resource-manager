using System;
using Moq;
using Shouldly;
using Xunit;
using LabManager.Services.Resources;
using LabManager.Common.Domain.Resource;

namespace LabManager.Services.Tests.Resource
{
    public class ResourceRepositoryExtensionsTests
    {
        [Fact]
        public void ResourceRespositoryExtensions_GetBy_ThrowsOnNull()
        {
            var repo = new Mock<IResourceRepository>();
            Should.Throw<InvalidOperationException>(()=> ResourceRepositoryExtensions.GetBy(repo.Object, null));
        }


        [Fact]
        public void ResourceRespositoryExtensions_GetBy_Launches_GetById()
        {
            var filter = new ResourceModel
            {
                Id = 123
            };
            var repo = new Mock<IResourceRepository>();
            repo.Object.GetBy(filter);

            repo.Verify(r=>r.GetById(It.Is<long>(l => l == filter.Id)), Times.Once);
        }

        [Fact]
        public void ResourceRespositoryExtensions_GetBy_Launches_GetByIpAddress()
        {
            var filter = new ResourceModel
            {
                IpAddress = "123"
            };
            var repo = new Mock<IResourceRepository>();
            repo.Object.GetBy(filter);

            repo.Verify(r => r.GetBy(It.Is<Func<ResourceModel, bool>>(f => f != null)), Times.Once);
        }

        [Fact]
        public void ResourceRespositoryExtensions_GetBy_BuildsFilter()
        {
            var filter = new ResourceModel
            {
                FriendlyName = "123"
            };
            var repo = new Mock<IResourceRepository>();
            repo.Object.GetBy(filter);

            repo.Verify(r => r.GetBy(It.Is<Func<ResourceModel, bool>>(f=> f!= null )), Times.Once);
        }

        [Fact]
        public void ResourceRespositoryExtensions_GetByIpAddress()
        {
            var repo = new Mock<IResourceRepository>();
            var ipAddress = "123";
            repo.Object.GetByIpAddress(ipAddress);

            repo.Verify(r => r.GetBy(It.Is<Func<ResourceModel, bool>>(f => f != null)), Times.Once);
        }
    }
}
