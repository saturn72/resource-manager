using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LabManager.IntegrationTests.Api
{
    public class RuntimeApiTests : IntegrationTestBase
    {
        internal override string ResourceUri => "/api/runtime/";

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
            //Create resource
            var createReq = BuildRequest(HttpMethod.Post, "api/resource", model);
            var createRes = await Client.SendAsync(createReq);
            createRes.EnsureSuccessStatusCode();

            //request for resource assignment
            var assignReqModel = new
            {
                requiredResources = new[]
                {
                    model
                },
                clientReferenceCode = "some-client-id"
            };
            var askForAssignReq = BuildRequest(HttpMethod.Post, ResourceUri, assignReqModel);
            var assignRes = await Client.SendAsync(askForAssignReq);
            assignRes.EnsureSuccessStatusCode();

            var jo = await ExtractJObject(assignRes);
            var sessionId = jo["sessionId"].Value<string>();
            
            //Validate count of returned resources
            var getUri = ResourceUri + sessionId;
            var confirmAssignReq = BuildRequest(HttpMethod.Get, getUri, null);
            var confirmAssignRes = await Client.SendAsync(confirmAssignReq);
            confirmAssignRes.EnsureSuccessStatusCode();


            //Check there is assigned resource resource
            var getResourcesRequest = BuildRequest(HttpMethod.Get, "api/resource", null);
            var getResourcesRes = await Client.SendAsync(getResourcesRequest);
            getResourcesRes.EnsureSuccessStatusCode();
            var allResources = await ExtractJArray(getResourcesRes);
            var activeResource = allResources.Where(r => r["active"].Value<bool>()).First();
            var activeResourceId = activeResource["id"].Value<long>();

            var isAssignedReq = BuildRequest(HttpMethod.Get, ResourceUri + "isassigned/" + activeResourceId, null);
            var isAssignedRes = await Client.SendAsync(isAssignedReq);
            isAssignedRes.EnsureSuccessStatusCode();

            throw new NotImplementedException("TODO: validate resource is assigned");

            //release resource here
            throw new NotImplementedException("TODO: releaswe resource");
            throw new NotImplementedException("TODO: releaswe resource VALIDATION");
        }
    }
}