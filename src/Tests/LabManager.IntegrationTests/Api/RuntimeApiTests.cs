using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
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

            //assignResource ==> Should fail as resource inactive
            var assignReq = BuildRequest(HttpMethod.Get, Resource, model);
            var assignRes = await Client.SendAsync(assignReq);
            assignRes.EnsureSuccessStatusCode();

            throw new NotImplementedException("approve session from here");
        }
    }
}