using Shouldly;
using Xunit;

namespace QAutomation.Extensions.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(default(string))]
        [InlineData("")]
        [InlineData("   ")]
        public void StringExtensions_HasValue_ReturnsFalse(string source)
        {
            source.HasValue().ShouldBeFalse();
        }

        [Fact]
        public void StringExtensions_HasValue_ReturnsTrue()
        {
            "test_string".HasValue().ShouldBeTrue();
        }
    }
}
