#region Usings

using QAutomation.Core.Domain;
using Shouldly;
using Xunit;

#endregion

namespace QAutomation.Core.Services.Tests
{
    public class ServiceResponseExtensionsTests
    {
        [Fact]
        public void ServiceResponseExtensions_HasError_ShouldReturnFalse()
        {
            var res = new ServiceResponse<SomeModel>(new SomeModel(), ServiceRequestType.Create);
            res.HasErrors().ShouldBeFalse();
        }

        [Fact]
        public void ServiceResponseExtensions_HasError_ShouldReturnTrue()
        {
            var res = new ServiceResponse<SomeModel>(new SomeModel(), ServiceRequestType.Create);
            res.ErrorMessage = "qwe";

            res.HasErrors().ShouldBeTrue();
        }

        internal class SomeModel : DomainModelBase
        {
        }
    }
}