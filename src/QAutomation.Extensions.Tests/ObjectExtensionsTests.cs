using QAutomation.Extensions;
using Shouldly;
using Xunit;

namespace QAutomation.Extensions.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void IsNull_ReturnsTrueOnNullObject()
        {
            ((object) null).IsNull().ShouldBeTrue();
        }

        [Fact]
        public void IsNull_ReturnsFalseOnReferencedObject()
        {
            new object().IsNull().ShouldBeFalse();
        }

        [Fact]
        public void NotNull_ReturnsFalseOnNullObject()
        {
            ((object) null).NotNull().ShouldBeFalse();
        }

        [Fact]
        public void NotNull_ReturnsTrueOnReferencedObject()
        {
            new object().NotNull().ShouldBeTrue();
        }
    }
}