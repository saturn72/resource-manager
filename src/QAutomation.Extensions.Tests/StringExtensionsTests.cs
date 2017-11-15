using System;
using System.Collections.Generic;
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
        [Fact]
        public void AsFormat_empty()
        {
            "    _logic".ShouldBe("    {0}".AsFormat("_logic"));
        }

        [Fact]
        public void AsFormat_MissingArgThrowsFormatException()
        {
            Should.Throw<FormatException>(() => "test{0} {1}".AsFormat(1));
        }

        [Fact]
        public void AsFormat_string_and_object()
        {
            var o = new object();
            "test_logic".ShouldBe("test{0}".AsFormat("_logic", o));
        }

        [Fact]
        public void AsFormat_strings()
        {
            "test_logic".ShouldBe("test{0}".AsFormat("_logic"));
        }

        [Fact]
        public void AsFormat_ThrowesException()
        {
            Should.Throw<FormatException>(() => "{ Test }".AsFormat("123"));
        }

        [Fact]
        public void AsFormat_Dictionary()
        {
            var formatDictionary = new Dictionary<string, object>
            {
                {"t1", "TTT"},
                {"t2", 2},
                {"t3", new object()}
            };

            "TTT 2 System.Object".ShouldBe("{t1} {t2} {t3}".AsFormat(formatDictionary));
        }
    }
}
