using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LabManager.IntegrationTests.Api
{
    public class RuntimeApiTests : IntegrationTestBase
    {
        internal override string Resource => "/api/runtime/";


        [Fact]
        public async Task RuntimeApi_AssignResource()
        {
            //create object
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss.fffff");
            var fName = "f_name" + timestamp;
            var ipAddress = "192.168.0.1" + timestamp;

            var model = new 
            {
                friendlyName = fName,
                ipAddress = ipAddress,
                active = true
            };
            var createReq = BuildRequest(HttpMethod.Post, "api/resource", model);
            var createRes = await Client.SendAsync(createReq);
            createRes.EnsureSuccessStatusCode();


            var assignReqModel = new
            {
                requiredResources = new[]
                {
                    model
                },
                clientReferenceCode = "some-client-id"
            };
            //assignResource ==> Should fail as resource inactive
            var askForAssignReq = BuildRequest(HttpMethod.Get, Resource, assignReqModel);
            var assignRes = await Client.SendAsync(askForAssignReq);
            assignRes.EnsureSuccessStatusCode();

            var jo = await ExtractJObject(assignRes);
            var sessionId = jo["sessionId"].Value<string>();
            
            //Validate count od returned resources
            var confirmAssignReq = BuildRequest(HttpMethod.Post, Resource, new{sessionId = sessionId});
            var confirmAssignRes = await Client.SendAsync(confirmAssignReq);
            confirmAssignRes.EnsureSuccessStatusCode();



            throw new NotImplementedException("TODO: validate number of resources returned form service (requested resources == assigned resourcesapprove session from here");
        }
    }
}