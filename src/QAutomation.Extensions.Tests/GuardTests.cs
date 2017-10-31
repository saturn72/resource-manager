using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace QAutomation.Extensions.Tests
{
    public class GuardTests
    {
        [Fact]
        public void Guard_GreaterThan()
        {
            //throws on not greater than
            Should.Throw<ArgumentOutOfRangeException>(() => Guard.GreaterThan(1, 1));
            Should.Throw<ArgumentOutOfRangeException>(() => Guard.GreaterOrEqualTo(1, 10));

            Guard.GreaterThan(1, 0);
        }

        [Fact]
        public void Guard_GreaterOrEqualsTo()
        {
            //throws on not greater than
            Should.Throw<ArgumentOutOfRangeException>(() => Guard.GreaterOrEqualTo(1, 10));

            Guard.GreaterOrEqualTo(1, 1);
            Guard.GreaterOrEqualTo(1, 0);
        }
        [Fact]
        public void Guard_SmallerThan()
        {
            //throws on not greater than
            Should.Throw<ArgumentOutOfRangeException>(() => Guard.SmallerThan(1, 1));
            Should.Throw<ArgumentOutOfRangeException>(() => Guard.SmallerThan(1, 0));

            Guard.SmallerThan(1, 10);
        }

        [Fact]
        public void Guard_SmallerOrEqualsTo()
        {
            //throws on not greater than
            Should.Throw<ArgumentOutOfRangeException>(() => Guard.SmallerOrEqualTo(10, 1));

            Guard.SmallerOrEqualTo(1, 1);
            Guard.SmallerOrEqualTo(1, 10);
        }

        [Fact]
        public void ContainsKey_KeyNotExists()
        {
            var dictionary = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 4 } };

            Assert.Throws<KeyNotFoundException>(() =>
                Guard.ContainsKey(dictionary, 9));
        }

        [Fact]
        public void ContainsKey_KeyExists()
        {
            var dictionary = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 4 } };
            Guard.ContainsKey(dictionary, 1);
        }

        [Fact]
        public void MustFollows_DoesNotTriggerAction()
        {
            var str = "test";
            Guard.MustFollow(str.Length == 4, () => str = str.ToUpper());
            "test".ShouldBe(str);
        }

        [Fact]
        public void MustFollows_TriggerAction()
        {
            var str = "test";
            Guard.MustFollow(() => str.Length == 0, () => str = str.ToUpper());
            "TEST".ShouldBe(str);
        }

        [Fact]
        public void NotEmpty_TriggersAction()
        {
            var x = 0;
            Guard.NotEmpty(new List<string>(), () => x++);
            1.ShouldBe(x);
        }

        [Fact]
        public void HasValue_DoesNotTriggersAction()
        {
            var x = 0;
            Guard.HasValue("test", () => x++);
            0.ShouldBe(x);
        }

        [Fact]
        public void HasValue_TriggersAction()
        {
            var x = 0;
            Guard.HasValue("", () => x++);
            1.ShouldBe(x);
        }

        [Fact]
        public void HasValue_ThrowsExceptionOnEmptyString()
        {
            Should.Throw<ArgumentNullException>(
                () => Guard.HasValue("", () => { throw new ArgumentNullException(); }));
        }

        [Fact]
        public void NotNull_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => Guard.NotNull((object)null));
        }

        [Fact]
        public void NotNull_ThrowsNullReferenceExceptionWithMessage()
        {
            Should.Throw<NullReferenceException>(() => Guard.NotNull((object)null, "message"),
                "message");
        }

        [Fact]
        public void NotNull_TriggerAction()
        {
            var x = 0;
            Guard.NotNull((object)null, () => x++);
            1.ShouldBe(x);
        }
    }
}