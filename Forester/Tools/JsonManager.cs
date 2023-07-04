using Newtonsoft.Json;

namespace Forester.Tools
{
    public static class JsonManager
    {
        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json)!;
        public static string Serialize(object obj) => JsonConvert.SerializeObject(obj);
    }
}
