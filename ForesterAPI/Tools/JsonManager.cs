using Newtonsoft.Json;

namespace ForesterAPI.Tools
{
    public static class JsonManager
    {
        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);
        public static string Serialize(object obj) => JsonConvert.SerializeObject(obj);
    }
}
