using Newtonsoft.Json.Linq;

namespace LabManager.WebService.Tests
{
    public class TestUtil
    {
        public static JObject ExtractJObject(object source)
        {
            return JObject.FromObject(source);
        }

        public static JArray ExtractJArray(object httpResponse)
        {
            return JArray.FromObject(httpResponse);
        }
    }
}