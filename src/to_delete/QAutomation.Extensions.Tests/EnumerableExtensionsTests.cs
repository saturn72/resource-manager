using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace QAutomation.Extensions.Tests
{
    public class EnumerableExtensionsTest
    {
        [Fact]
        public void EnumerableExtensions_ForEachItem_IterateAll()
        {
            var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var dest = new List<int>();

            source.ForEachItem(i => dest.Add(i));

            dest.Count.ShouldBe(source.Length);
            for (var i = 0; i < dest.Count; i++)
                dest.ElementAt(i).ShouldBe(source[i]);
        }

        [Fact]
        public void EnumerableExtensions_IsEmptyOrNull_returnsTrueCases()
        {
            new List<string>().IsEmptyOrNull().ShouldBeTrue();

            ((IEnumerable<string>)null).IsEmptyOrNull().ShouldBeTrue();
            "".IsEmptyOrNull().ShouldBeTrue();
        }

        [Fact]
        public void EnumerableExtensions_IsEmptyOrNull_ReturnsFalseCases()
        {
            "Test".IsEmptyOrNull().ShouldBeFalse();
        }
    }
}
