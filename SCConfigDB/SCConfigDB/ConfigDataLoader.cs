using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Defter.StarCitizen.ConfigDB.Json;
using Defter.StarCitizen.ConfigDB.Model;

namespace Defter.StarCitizen.ConfigDB
{
    public abstract class ConfigDataLoader
    {
        protected ConfigDataJsonNode? DatabaseJsonNode { get; set; }
        protected Dictionary<string, ConfigDataTranslateJsonNode> TranslateJsonNodes { get; } =
            new Dictionary<string, ConfigDataTranslateJsonNode>();

        public ConfigDataLoader() { }

        public abstract Task LoadDatabaseAsync(bool forceReload = false);

        public abstract Task LoadTranslationAsync(string language, bool forceReload = false);

        public void LoadDatabase(bool forceReload = false) =>
            LoadDatabaseAsync(forceReload).GetAwaiter().GetResult();

        public void LoadTranslation(string language, bool forceReload = false) =>
            LoadTranslationAsync(language, forceReload).GetAwaiter().GetResult();

        public void LoadAll(string? language = null, bool forceReload = false)
        {
            if (language == null)
            {
                LoadDatabase(forceReload);
                return;
            }
            Task[] tasks = new Task[2];
            tasks[0] = LoadDatabaseAsync(forceReload);
            tasks[1] = LoadTranslationAsync(language, forceReload);
            Task.WaitAll(tasks);
        }

        public ConfigData BuildData(string? language)
        {
            if (DatabaseJsonNode == null)
                throw new InvalidOperationException("database not loaded to build config data");
            if (language != null && TranslateJsonNodes.TryGetValue(language, out var translateJsonNode))
            {
                return ConfigDatabase.Build(DatabaseJsonNode, translateJsonNode);
            }
            return ConfigDatabase.Build(DatabaseJsonNode);
        }
    }

    public sealed class LocalConfigDataLoader : ConfigDataLoader
    {
        private readonly string _databasePath;

        public LocalConfigDataLoader(string path)
        {
            _databasePath = path;
        }

        public override async Task LoadDatabaseAsync(bool forceReload = false)
        {
            if (forceReload || DatabaseJsonNode == null)
            {
                DatabaseJsonNode = await ConfigDatabase.LoadFromFileAsync(
                    LocalSourceSettings.DatabaseFilePath(_databasePath));
            }
        }

        public override async Task LoadTranslationAsync(string language, bool forceReload = false)
        {
            if (forceReload || !TranslateJsonNodes.ContainsKey(language))
            {
                TranslateJsonNodes[language] = await ConfigDatabase.LoadTranslateFromFileAsync(
                    LocalSourceSettings.DatabaseTranslateFilePath(_databasePath, language));
            }
        }
    }

    public sealed class NetworkConfigDataLoader : ConfigDataLoader
    {
        private readonly HttpClient _client;
        private readonly GitSourceSettings _sourceSettings;

        public NetworkConfigDataLoader(HttpClient client, GitSourceSettings sourceSettings)
        {
            _client = client;
            _sourceSettings = sourceSettings;
        }

        public override async Task LoadDatabaseAsync(bool forceReload = false)
        {
            if (forceReload || DatabaseJsonNode == null)
            {
                DatabaseJsonNode = await ConfigDatabase.LoadFromUrlAsync(_client,
                    _sourceSettings.DatabaseUrl, null);
            }
        }

        public override async Task LoadTranslationAsync(string language, bool forceReload = false)
        {
            if (forceReload || !TranslateJsonNodes.ContainsKey(language))
            {
                TranslateJsonNodes[language] = await ConfigDatabase.LoadTranslateFromUrlAsync(
                    _client, _sourceSettings.DatabaseTranslateUrl(language), null);
            }
        }
    }
}
