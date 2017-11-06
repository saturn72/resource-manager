
using System;
using Moq;
using QAutomation.Core.Services.Caching;
using Shouldly;
using Xunit;

namespace QAutomation.Core.Services.Tests.Caching
{
    public class CacheManagerExtensionsTests
    {
        [Fact]
        public void CacheManagerExtensions_Set()
        {
            var cm = new Mock<ICacheManager>();
            var key = "key";
            var value = new CacheTestObject();
            CacheManagerExtensions.Set(cm.Object, key, value);
            cm.Verify(c => c.Set(
                It.Is<string>(k => k == key),
                It.Is<CacheTestObject>(cto => cto == value),
                It.Is<int>(i => i == CacheManagerExtensions.DefaultCacheTime)), Times.Once);
        }

        [Fact]
        public void CacheManagerExtensions_SetIfNotExists()
        {
            var cm = new Mock<ICacheManager>();

            var key = "key";
            var value = new CacheTestObject();
            var cacheTime = 123;

            //object is cached
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(null as CacheTestObject);

            CacheManagerExtensions.SetIfNotExists(cm.Object, key, value, cacheTime);
            cm.Verify(c => c.Set(
                    It.Is<string>(k => k == key),
                    It.Is<CacheTestObject>(cto => cto == value),
                    It.IsAny<int>()),
                Times.Once);

            //object is not cached
            cm.Reset();
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(new CacheTestObject());
            CacheManagerExtensions.SetIfNotExists(cm.Object, key, value, cacheTime);
            cm.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<CacheTestObject>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public void CacheManagerExtensions_GetOrAquire()
        {
            var cm = new Mock<ICacheManager>();
            var key = "key";
            var wasAquired = false;
            Func<CacheTestObject> aquireFunc = () =>
            {
                wasAquired = true;
                return new CacheTestObject();
            };
            //does not call aquire func
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(new CacheTestObject());

            CacheManagerExtensions.Get(cm.Object, key, aquireFunc);
            wasAquired.ShouldBeFalse();

            //should call call aquire func
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(null as CacheTestObject);
            CacheManagerExtensions.Get(cm.Object, key, aquireFunc);
            wasAquired.ShouldBeTrue();
        }

        internal class CacheTestObject
        {

        }
    }
}
