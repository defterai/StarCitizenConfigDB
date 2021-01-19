using Defter.StarCitizen.ConfigDB.Json;
using Defter.StarCitizen.ConfigDB.Model;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Defter.StarCitizen.ConfigDB
{
    public static class ConfigDatabase
    {
        private static readonly ParameterFactory _parameterFactory = new ParameterFactory();
        private static readonly SettingFactory _settingFactory = new SettingFactory();

        public static Encoding JsonEncoding { get; } = Encoding.UTF8;

        // Loading (json -> json nodes -> model)

        public static T LoadFromString<T>(string json) =>
            DbJsonConverter.Deserialize<T>(json);

        public static T LoadFromFile<T>(string path) =>
            LoadFromString<T>(File.ReadAllText(path, JsonEncoding));

        public static async Task<T> LoadFromFileAsync<T>(string path) =>
            LoadFromString<T>(await GetContentFromFileAsync(path, JsonEncoding).ConfigureAwait(false));

        public static async Task<T> LoadFromUrlAsync<T>(HttpClient client, string url, CancellationToken? cancellationToken = default) =>
            LoadFromString<T>(await GetContentFromUrlAsync(client, url, cancellationToken).ConfigureAwait(false));

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

        // Saving (model -> json nodes -> json)

        public static string SaveToString<T>(T node) =>
            DbJsonConverter.Serialize(node);

        public static void SaveToFile<T>(T node, string path) =>
            File.WriteAllText(path, SaveToString(node), JsonEncoding);

        public static async Task SaveToFileAsync<T>(T node, string path) =>
            await WriteContentToFileAsync(path, SaveToString(node), JsonEncoding).ConfigureAwait(false);

        public static ConfigDataJsonNode BuildNode(ConfigData data, ISet<string> languages,
            INetworkSourceSettings? schemaSource = null)
        {
            // build commands node
            var commandsJsonBuilder = new CommandsJsonNode.Builder();
            foreach (var commandCategoryPair in data.CommandCategories)
            {
                var categoryBuilder = new CategoryJsonNode.Builder(commandCategoryPair.Key,
                    commandCategoryPair.Value.Name);
                commandsJsonBuilder.Categories.Add(categoryBuilder.Build());
                foreach (var commandPair in commandCategoryPair.Value.Commands)
                {
                    var command = commandPair.Value;
                    var commandBuilder = new CommandJsonNode.Builder(command.Key,
                        command.Name, commandCategoryPair.Key);
                    commandBuilder.Description = command.Description;
                    if (command.Parameters != null)
                    {
                        foreach (var parameter in command.Parameters)
                        {
                            var paramBuilder = new ParamJsonNode.Builder(parameter.Name,
                                parameter.GetValuesNode());
                            paramBuilder.Description = parameter.Description;
                            commandBuilder.Parameters.Add(paramBuilder.Build());
                        }
                    }
                    commandsJsonBuilder.Items.Add(commandBuilder.Build());
                }
            }
            // build settings node
            var settingsJsonBuilder = new SettingsJsonNode.Builder();
            foreach (var settingCategoryPair in data.SettingCategories)
            {
                var categoryBuilder = new CategoryJsonNode.Builder(settingCategoryPair.Key,
                    settingCategoryPair.Value.Name);
                settingsJsonBuilder.Categories.Add(categoryBuilder.Build());
                foreach (var settingPair in settingCategoryPair.Value.Settings)
                {
                    var setting = settingPair.Value;
                    var settingBuilder = new SettingJsonNode.Builder(setting.Key,
                        setting.Name, settingCategoryPair.Key, setting.GetValuesNode());
                    settingBuilder.Description = setting.Description;
                    settingsJsonBuilder.Items.Add(settingBuilder.Build());
                }
            }
            // build final root node
            var builder = new ConfigDataJsonNode.Builder(commandsJsonBuilder.Build(),
                settingsJsonBuilder.Build());
            if (schemaSource != null)
            {
                builder.Schema = schemaSource.JsonSchemaUrl;
            }
            builder.Languages.UnionWith(languages);
            return builder.Build();
        }

        public static ConfigDataTranslateJsonNode BuildTranslateNode(ConfigData data, INetworkSourceSettings? schemaSource = null)
        {
            // build commands node
            var commandsJsonBuilder = new CommandsTranslateJsonNode.Builder();
            foreach (var commandCategoryPair in data.CommandCategories)
            {
                var categoryBuilder = new CategoryJsonNode.Builder(commandCategoryPair.Key,
                    commandCategoryPair.Value.Name);
                commandsJsonBuilder.Categories.Add(categoryBuilder.Build());
                foreach (var commandPair in commandCategoryPair.Value.Commands)
                {
                    var command = commandPair.Value;
                    var commandBuilder = new CommandTranslateJsonNode.Builder(command.Key, command.Name);
                    commandBuilder.Description = command.Description;
                    if (command.Parameters != null)
                    {
                        foreach (var parameter in command.Parameters)
                        {
                            var paramBuilder = new ParamTranslateJsonNode.Builder(parameter.Name);
                            parameter.ExctractValueNodes(paramBuilder.Values);
                            paramBuilder.Description = parameter.Description;
                            commandBuilder.Parameters.Add(paramBuilder.Build());
                        }
                    }
                    commandsJsonBuilder.Items.Add(commandBuilder.Build());
                }
            }
            // build settings node
            var settingsJsonBuilder = new SettingsTranslateJsonNode.Builder();
            foreach (var settingCategoryPair in data.SettingCategories)
            {
                var categoryBuilder = new CategoryJsonNode.Builder(settingCategoryPair.Key,
                    settingCategoryPair.Value.Name);
                settingsJsonBuilder.Categories.Add(categoryBuilder.Build());
                foreach (var settingPair in settingCategoryPair.Value.Settings)
                {
                    var setting = settingPair.Value;
                    var settingBuilder = new SettingTranslateJsonNode.Builder(setting.Key, setting.Name);
                    setting.ExctractValueNodes(settingBuilder.Values);
                    settingBuilder.Description = setting.Description;
                    settingsJsonBuilder.Items.Add(settingBuilder.Build());
                }
            }
            // build final root node
            var builder = new ConfigDataTranslateJsonNode.Builder(commandsJsonBuilder.Build(),
                settingsJsonBuilder.Build());
            if (schemaSource != null)
            {
                builder.Schema = schemaSource.JsonTranslateSchemaUrl;
            }
            return builder.Build();
        }

        public static async Task WriteContentToFileAsync(string path, string content, Encoding encoding)
        {
            using var streamWriter = new StreamWriter(new FileStream(path, FileMode.Create), encoding);
            await streamWriter.WriteAsync(content).ConfigureAwait(false);
        }
    }
}
