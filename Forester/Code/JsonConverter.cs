using System.Text.Json;

namespace Forester
{ 
    /// <summary>
    /// Tool class for json managment
    /// </summary>
    public static class JsonConverter
    {
        /// <summary>
        /// Serialize object to json
        /// </summary>
        public static string Serialize(object text) =>
            JsonSerializer.Serialize(text, text.GetType());

        /// <summary>
        /// deserialize json to object
        /// </summary>
        public static T Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json);
    }
}
