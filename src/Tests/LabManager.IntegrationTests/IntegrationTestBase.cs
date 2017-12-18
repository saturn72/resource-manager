using System.IO;
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
        private const string DbPath = "resource-manager-test.db";

        protected readonly HttpClient Client;
        protected readonly TestServer Server;

        protected IntegrationTestBase()
        {
            if (File.Exists(DbPath))
                File.Delete(DbPath);

            Server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        internal abstract string ResourceUri { get; }

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
            return ExtractJObject(await ContentToString(httpResponse));
        }

        protected static JObject ExtractJObject(object obj)
        {
            return obj is string ? JObject.Parse(obj as string) : JObject.FromObject(obj);
        }


        protected async Task<JArray> ExtractJArray(HttpResponseMessage httpResponse)
        {
            return ExtractJArray(await ContentToString(httpResponse));
        }

        protected static JArray ExtractJArray(object obj)
        {
            return obj is string ? JArray.Parse(obj as string) : JArray.FromObject(obj);
        }

        private async Task<string> ContentToString(HttpResponseMessage httpResponse)
        {
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}