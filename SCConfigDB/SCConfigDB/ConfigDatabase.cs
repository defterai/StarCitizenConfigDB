using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Defter.StarCitizen.ConfigDB.Json;
using Defter.StarCitizen.ConfigDB.Model;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace Defter.StarCitizen.ConfigDB
{
    public static class ConfigDatabase
    {
        private static readonly JsonSerializerSettings _jsonSettings = GetJsonSettings();
        private static readonly ParameterFactory _parameterFactory = new ParameterFactory();
        private static readonly SettingFactory _settingFactory = new SettingFactory();

        private static JsonSerializerSettings GetJsonSettings()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            settings.Converters.Add(new StringEnumConverter(new SnakeCaseNamingStrategy(), false));
            return settings;
        }

        public static ConfigDataJsonNode LoadFromString(string json)
        {
            var node = JsonConvert.DeserializeObject<ConfigDataJsonNode>(json, _jsonSettings);
            if (node == null)
                throw new InvalidDataException("database json data is invalid");
            return node;
        }

        public static ConfigDataTranslateJsonNode LoadTranslateFromString(string json)
        {
            var node = JsonConvert.DeserializeObject<ConfigDataTranslateJsonNode>(json, _jsonSettings);
            if (node == null)
                throw new InvalidDataException("database translate json data is invalid");
            return node;
        }

        public static ConfigDataJsonNode LoadFromFile(string path) =>
            LoadFromString(File.ReadAllText(path, Encoding.UTF8));

        public static ConfigDataTranslateJsonNode LoadTranslateFromFile(string path) =>
            LoadTranslateFromString(File.ReadAllText(path, Encoding.UTF8));

        public static async Task<ConfigDataJsonNode> LoadFromFileAsync(string path) =>
            LoadFromString(await GetContentFromFileAsync(path, Encoding.UTF8).ConfigureAwait(false));

        public static async Task<ConfigDataTranslateJsonNode> LoadTranslateFromFileAsync(string path) =>
            LoadTranslateFromString(await GetContentFromFileAsync(path, Encoding.UTF8).ConfigureAwait(false));

        public static async Task<ConfigDataJsonNode> LoadFromUrlAsync(HttpClient client, string url, CancellationToken? cancellationToken = default) =>
            LoadFromString(await GetContentFromUrlAsync(client, url, cancellationToken).ConfigureAwait(false));

        public static async Task<ConfigDataTranslateJsonNode> LoadTranslateFromUrlAsync(HttpClient client, string url, CancellationToken? cancellationToken = default) =>
            LoadTranslateFromString(await GetContentFromUrlAsync(client, url, cancellationToken).ConfigureAwait(false));

        public static ConfigData Build(ConfigDataJsonNode node)
        {
            var builder = new ConfigData.Builder(node, _settingFactory, _parameterFactory);
            return builder.Build();
        }

        public static ConfigData Build(ConfigDataJsonNode node, ConfigDataTranslateJsonNode translateNode) =>
            Build(node.TranslateWith(translateNode));

        public static async Task<string> GetContentFromFileAsync(string path, Encoding encoding)
        {
            using var streamReader = new StreamReader(path, encoding);
            return await streamReader.ReadToEndAsync().ConfigureAwait(false);
        }

        public static async Task<string> GetContentFromUrlAsync(HttpClient client, string url, CancellationToken? cancellationToken = default)
        {
            using var result = cancellationToken.HasValue ?
                    await client.GetAsync(url, cancellationToken.Value).ConfigureAwait(false) :
                    await client.GetAsync(url).ConfigureAwait(false);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
