using System.Threading.Tasks;
using Xunit;

namespace LabManager.IntegrationTests.Api
{
    public class RuntimeApiTests : IntegrationTestBase
    {
        internal override string Resource => "/api/runtime/";


        [Fact]
        public async Task RuntimeApi_AssignResource()
        {
            //Gets any pump
            var res =  await Client.GetAsync(Resource);
            res.EnsureSuccessStatusCode();
        }
    }
}