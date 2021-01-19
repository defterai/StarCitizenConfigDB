using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public static class DbJsonConverter
    {
        private static readonly JsonSerializerSettings _jsonSettings = GetJsonSettings();

        private static JsonSerializerSettings GetJsonSettings()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            settings.Converters.Add(new StringEnumConverter(new SnakeCaseNamingStrategy(), false));
            return settings;
        }

        public static T Deserialize<T>(string json)
        {
            var node = JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            if (node == null)
                throw new InvalidDataException("json data is invalid");
            return node;
        }

        public static string Serialize<T>(T node)
        {
            using var stringWriter = new StringWriter();
            using var writer = new DbJsonTextWriter(stringWriter);
            JsonSerializer serializer = JsonSerializer.Create(_jsonSettings);
            serializer.Serialize(writer, node);
            return stringWriter.ToString();
        }
    }
}
