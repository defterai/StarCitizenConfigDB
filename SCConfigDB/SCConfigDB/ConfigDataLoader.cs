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
            new Dictionary<string, ConfigDataTranslateJsonNode>(StringComparer.OrdinalIgnoreCase);

        public ICollection<string> LoadedLanguages => TranslateJsonNodes.Keys;

        public abstract Task LoadDatabaseAsync(bool forceReload = false);

        public abstract Task LoadTranslationAsync(string language, bool forceReload = false);

        public void LoadDatabase(bool forceReload = false) =>
            LoadDatabaseAsync(forceReload).GetAwaiter().GetResult();

        public void LoadTranslation(string language, bool forceReload = false) =>
            LoadTranslationAsync(language, forceReload).GetAwaiter().GetResult();

        public ISet<string> GetSupportedLanguages()
        {
            if (DatabaseJsonNode == null)
                throw new InvalidOperationException("database not loaded to get supported languages");
            return DatabaseJsonNode.Languages;
        }

        public ConfigData BuildData(string? language = null)
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

    public sealed class FileConfigDataLoader : ConfigDataLoader
    {
        private readonly string _databasePath;

        public FileConfigDataLoader(string path)
        {
            _databasePath = path;
        }

        public override async Task LoadDatabaseAsync(bool forceReload = false)
        {
            if (forceReload || DatabaseJsonNode == null)
            {
                DatabaseJsonNode = await ConfigDatabase.LoadFromFileAsync(
                    FileSourceSettings.DatabaseFilePath(_databasePath));
            }
        }

        public override async Task LoadTranslationAsync(string language, bool forceReload = false)
        {
            if (forceReload || !TranslateJsonNodes.ContainsKey(language))
            {
                TranslateJsonNodes[language] = await ConfigDatabase.LoadTranslateFromFileAsync(
                    FileSourceSettings.DatabaseTranslateFilePath(_databasePath, language));
            }
        }
    }

    public sealed class GitHubConfigDataLoader : ConfigDataLoader
    {
        private readonly HttpClient _client;
        private readonly GitHubSourceSettings _sourceSettings;

        public GitHubConfigDataLoader(HttpClient client) : this(client, new GitHubSourceSettings()) { }

        public GitHubConfigDataLoader(HttpClient client, GitHubSourceSettings sourceSettings)
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
