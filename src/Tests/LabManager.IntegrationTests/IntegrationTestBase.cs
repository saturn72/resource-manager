using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LabManager.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;

namespace LabManager.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        internal abstract string Resource { get; }
        protected IntegrationTestBase()
        {
            Server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        protected readonly HttpClient Client;
        protected readonly TestServer Server;

        protected HttpRequestMessage BuildRequest(HttpMethod httpMethod, string relativeUri, object content)
        {
            HttpContent requestContent = null;

            if (content != null)
            {
                var model = JObject.FromObject(content);
                requestContent = new StringContent(model.ToString(), Encoding.UTF8, MediaTypes.ApplicationJson);
            }

            return new HttpRequestMessage(httpMethod, relativeUri)
            {
                Content = requestContent
            };
        }

        protected async Task<JObject> ExtractJObject(HttpResponseMessage httpResponse)
        {
            var content = await ContentToString(httpResponse);

            return JObject.Parse(content);
        }

        protected async Task<JArray> ExtractJArray(HttpResponseMessage httpResponse)
        {
            var content = await ContentToString(httpResponse);
            return JArray.Parse(content);
        }

        private async Task<string> ContentToString(HttpResponseMessage httpResponse)
        {
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}
