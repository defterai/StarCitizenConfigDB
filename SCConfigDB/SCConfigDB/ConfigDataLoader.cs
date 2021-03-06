using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
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

        public bool DatabaseLoaded => DatabaseJsonNode != null;
        public ICollection<string> LoadedLanguages => TranslateJsonNodes.Keys;

        public abstract Task LoadDatabaseAsync(CancellationToken? cancellationToken = default, bool forceReload = false);

        public abstract Task LoadTranslationAsync(string language, CancellationToken? cancellationToken = default, bool forceReload = false);

        public void LoadDatabase(bool forceReload = false) =>
            LoadDatabaseAsync(default, forceReload).GetAwaiter().GetResult();

        public void LoadTranslation(string language, bool forceReload = false) =>
            LoadTranslationAsync(language, default, forceReload).GetAwaiter().GetResult();

        public ISet<string> GetSupportedLanguages()
        {
            if (DatabaseJsonNode == null)
                throw new InvalidOperationException("database not loaded to get supported languages");
            return DatabaseJsonNode.Languages;
        }

        public void UnloadDatabase() => DatabaseJsonNode = null;

        public bool UnloadTranslation(string? language = null)
        {
            if (language != null)
            {
                return TranslateJsonNodes.Remove(language);
            }
            TranslateJsonNodes.Clear();
            return true;
        }

        public void UnloadAll()
        {
            UnloadDatabase();
            UnloadTranslation();
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
        private readonly IFileSourceSettings _sourceSettings;

        public FileConfigDataLoader(IFileSourceSettings sourceSettings)
        {
            _sourceSettings = sourceSettings;
        }

        public override async Task LoadDatabaseAsync(CancellationToken? cancellationToken = default, bool forceReload = false)
        {
            if (forceReload || DatabaseJsonNode == null)
            {
                DatabaseJsonNode = await ConfigDatabase.LoadFromFileAsync<ConfigDataJsonNode>(
                    _sourceSettings.DatabaseFilePath);
            }
        }

        public override async Task LoadTranslationAsync(string language, CancellationToken? cancellationToken = default, bool forceReload = false)
        {
            if (forceReload || !TranslateJsonNodes.ContainsKey(language))
            {
                TranslateJsonNodes[language] = await ConfigDatabase.LoadFromFileAsync<ConfigDataTranslateJsonNode>(
                    _sourceSettings.DatabaseTranslateFilePath(language));
            }
        }
    }

    public sealed class NetworkConfigDataLoader : ConfigDataLoader
    {
        private readonly HttpClient _client;
        private readonly INetworkSourceSettings _sourceSettings;

        public NetworkConfigDataLoader(HttpClient client, INetworkSourceSettings sourceSettings)
        {
            _client = client;
            _sourceSettings = sourceSettings;
        }

        public override async Task LoadDatabaseAsync(CancellationToken? cancellationToken = default, bool forceReload = false)
        {
            if (forceReload || DatabaseJsonNode == null)
            {
                DatabaseJsonNode = await ConfigDatabase.LoadFromUrlAsync<ConfigDataJsonNode>(_client,
                    _sourceSettings.DatabaseUrl, cancellationToken);
            }
        }

        public override async Task LoadTranslationAsync(string language, CancellationToken? cancellationToken = default, bool forceReload = false)
        {
            if (forceReload || !TranslateJsonNodes.ContainsKey(language))
            {
                TranslateJsonNodes[language] = await ConfigDatabase.LoadFromUrlAsync<ConfigDataTranslateJsonNode>(
                    _client, _sourceSettings.DatabaseTranslateUrl(language), cancellationToken);
            }
        }
    }
}
