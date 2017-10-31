using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LabManager.IntegrationTests.Api
{
    public class ResourceApiTests:IntegrationTestBase
    {
        internal override string Resource => "/api/resource/";


        [Fact]
        public async Task ResourceApi_Create_AndGetById_AndGetAll()
        {
            var fName = "f_name" + DateTime.UtcNow.ToString("yyyy-MM-dd_hh-mm-ss.fffff");
            var ipAddress = "192.168.0.1";

            var model = new
            {
                friendlyName = fName,
                ipAddress = ipAddress
            };
            var createReq = BuildRequest(HttpMethod.Post, Resource, model);
            var createRes = await Client.SendAsync(createReq);
            createRes.EnsureSuccessStatusCode();
            var jo = await ExtractJObject(createRes);
            var id = (long) jo["id"];
            id.ShouldBeGreaterThan(0);

            //GetById
            var getByIdRes = await Client.GetAsync(Resource + id);
            getByIdRes.EnsureSuccessStatusCode();

            var getByIdResJO = await ExtractJObject(getByIdRes);
            getByIdResJO["friendlyName"].ToString().ShouldBe(fName);
            getByIdResJO["ipAddress"].ToString().ShouldBe(ipAddress);

            //GetAll
            var getAllRes = await Client.GetAsync(Resource);
            getAllRes.EnsureSuccessStatusCode();

            var ja = await ExtractJArray(getAllRes);
            ja.Count.ShouldBeGreaterThan(0);
        }
    }
}
