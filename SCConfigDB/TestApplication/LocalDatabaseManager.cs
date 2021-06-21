using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Defter.StarCitizen.ConfigDB;
using Defter.StarCitizen.ConfigDB.Json;
using Defter.StarCitizen.ConfigDB.Model;
using Defter.StarCitizen.ConfigDB.Transaction;

namespace Defter.StarCitizen.TestApplication
{
    public interface IErrorOutput
    {
        void WriteLine(string value);
    }

    public sealed class LocalDatabaseManager
    {
        private readonly INetworkSourceSettings _networkSourceSettings;
        private readonly IFileSourceSettings _fileSourceSettings;
        private readonly IErrorOutput _errorOutput;
        private readonly HttpClient _httpClient;
        private ConfigDataJsonNode? _configDataJsonNode;
        private readonly Dictionary<string, ConfigDataTranslateJsonNode> _configDataTranslateJsonNodes =
            new Dictionary<string, ConfigDataTranslateJsonNode>(StringComparer.OrdinalIgnoreCase);
        private ConfigData? _configData;


        public LocalDatabaseManager(INetworkSourceSettings networkSourceSettings, IFileSourceSettings fileSourceSettings, IErrorOutput errorOutput)
        {
            _networkSourceSettings = networkSourceSettings;
            _fileSourceSettings = fileSourceSettings;
            _errorOutput = errorOutput;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var clientHandler = new HttpClientHandler
            {
                UseProxy = false
            };
            _httpClient = new HttpClient(clientHandler);
            var assemlyName = Assembly.GetExecutingAssembly().GetName();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{assemlyName.Name}/{assemlyName.Version.ToString(3)}");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public bool Unload()
        {
            _configDataJsonNode = null;
            _configDataTranslateJsonNodes.Clear();
            _configData = null;
            return true;
        }

        public bool LoadLocal()
        {
            Unload();
            try
            {
                _configDataJsonNode = ConfigDatabase.LoadFromFile<ConfigDataJsonNode>(_fileSourceSettings.DatabaseFilePath);
                foreach (var language in _configDataJsonNode.Languages)
                {
                    var path = _fileSourceSettings.DatabaseTranslateFilePath(language);
                    _configDataTranslateJsonNodes.Add(language, ConfigDatabase.LoadFromFile<ConfigDataTranslateJsonNode>(path));
                }
                _configData = ConfigDatabase.Build(_configDataJsonNode);
            }
            catch (Exception e)
            {
                Unload();
                _errorOutput.WriteLine("Error: Failed load database - " + e.Message);
                return false;
            }
            return true;
        }

        public bool LoadNetwork()
        {
            Unload();
            try
            {
                _configDataJsonNode = ConfigDatabase.LoadFromUrlAsync<ConfigDataJsonNode>(_httpClient,
                    _networkSourceSettings.DatabaseUrl).GetAwaiter().GetResult();
                foreach (var language in _configDataJsonNode.Languages)
                {
                    var url = _networkSourceSettings.DatabaseTranslateUrl(language);
                    _configDataTranslateJsonNodes.Add(language,
                        ConfigDatabase.LoadFromUrlAsync<ConfigDataTranslateJsonNode>(_httpClient, url).GetAwaiter().GetResult());
                }
                _configData = ConfigDatabase.Build(_configDataJsonNode);
            }
            catch (Exception e)
            {
                Unload();
                _errorOutput.WriteLine("Error: Failed load database - " + e.Message);
                return false;
            }
            return true;
        }

        public bool Init()
        {
            if (_configData != null || _configDataJsonNode != null)
            {
                _errorOutput.WriteLine("Error: Database already loaded");
                return false;
            }
            _configDataJsonNode = ConfigDatabase.CreateNode(_networkSourceSettings);
            _configData = ConfigDatabase.Build(_configDataJsonNode);
            return true;
        }

        public bool Save()
        {
            if (_configData == null || _configDataJsonNode == null)
            {
                _errorOutput.WriteLine("Error: Database not loaded");
                return false;
            }
            ConfigDatabase.SaveToFile(_configDataJsonNode, _fileSourceSettings.DatabaseFilePath);
            foreach (var pair in _configDataTranslateJsonNodes)
            {
                var path = _fileSourceSettings.DatabaseTranslateFilePath(pair.Key);
                ConfigDatabase.SaveToFile(pair.Value, path);
            }
            return true;
        }

        public ConfigData? GetData() => _configData;

        public ConfigData? GetLanguageData(string language)
        {
            if (_configData == null || _configDataJsonNode == null)
            {
                _errorOutput.WriteLine("Error: Database not loaded");
                return null;
            }
            if (!_configDataTranslateJsonNodes.TryGetValue(language, out var translationJsonNode))
            {
                _errorOutput.WriteLine("Error: Language not found");
                return null;
            }
            return ConfigDatabase.Build(_configDataJsonNode, translationJsonNode);
        }

        public bool CreateLanguage(string language)
        {
            if (_configData == null || _configDataJsonNode == null ||
                _configDataJsonNode.Languages.Contains(language))
            {
                _errorOutput.WriteLine("Error: Database not loaded");
                return false;
            }
            if (_configDataJsonNode.Languages.Contains(language))
            {
                _errorOutput.WriteLine("Error: Language name already exist");
                return false;
            }
            string translationJsonPath = _fileSourceSettings.DatabaseTranslateFilePath(language);
            if (File.Exists(translationJsonPath))
            {
                _errorOutput.WriteLine("Error: Language folder already exist");
                return false;
            }
            var newTranslationJsonNode = ConfigDatabase.BuildTranslateNode(_configData, _networkSourceSettings);

            ConfigDataJsonNode.Builder builder = new ConfigDataJsonNode.Builder(_configDataJsonNode);
            builder.Languages.Add(language);
            var updatedJsonNode = builder.Build();

            Directory.CreateDirectory(Path.GetDirectoryName(translationJsonPath));
            using var trasactionGroup = new TransactionGroup(2);
            trasactionGroup.Add(ConfigSaveTransaction.Create(newTranslationJsonNode, translationJsonPath));
            trasactionGroup.Add(ConfigSaveTransaction.Create(updatedJsonNode, _fileSourceSettings.DatabaseFilePath));
            if (!trasactionGroup.Apply())
            {
                _errorOutput.WriteLine("Error: Failed write language files");
                return false;
            }
            _configDataTranslateJsonNodes[language] = newTranslationJsonNode;
            _configDataJsonNode = updatedJsonNode;
            trasactionGroup.Commit();
            return true;
        }

        public bool AddLanguage(string language)
        {
            if (_configData == null || _configDataJsonNode == null)
            {
                _errorOutput.WriteLine("Error: Database not loaded");
                return false;
            }
            if (_configDataJsonNode.Languages.Contains(language))
            {
                _errorOutput.WriteLine("Error: Language alreaady exist");
                return false;
            }
            string translationJsonPath = _fileSourceSettings.DatabaseTranslateFilePath(language);
            var newTranslationJsonNode = ConfigDatabase.LoadFromFile<ConfigDataTranslateJsonNode>(translationJsonPath);

            ConfigDataJsonNode.Builder builder = new ConfigDataJsonNode.Builder(_configDataJsonNode);
            builder.Languages.Add(language);
            var updatedJsonNode = builder.Build();

            using var transation = ConfigSaveTransaction.Create(updatedJsonNode, _fileSourceSettings.DatabaseFilePath);
            if (!transation.Apply())
            {
                _errorOutput.WriteLine("Error: Failed add language to list");
                return false;
            }
            _configDataTranslateJsonNodes[language] = newTranslationJsonNode;
            _configDataJsonNode = updatedJsonNode;
            transation.Commit();
            return true;
        }

        public bool UpdateLanguage(string language)
        {
            if (_configData == null || _configDataJsonNode == null)
            {
                _errorOutput.WriteLine("Error: Database not loaded");
                return false;
            }
            if (!_configDataJsonNode.Languages.Contains(language))
            {
                _errorOutput.WriteLine("Error: Language not found");
                return false;
            }

            if (!_configDataTranslateJsonNodes.TryGetValue(language, out var translationJsonNode))
            {
                _errorOutput.WriteLine("Error: Language json not found");
                return false;
            }
            var updatedJsonNode = ConfigDatabase.Build(_configDataJsonNode, translationJsonNode);
            var updatedTranslationJsonNode = ConfigDatabase.BuildTranslateNode(updatedJsonNode, _networkSourceSettings);
            using var transation = ConfigSaveTransaction.Create(updatedTranslationJsonNode,
                _fileSourceSettings.DatabaseTranslateFilePath(language));
            if (!transation.Apply())
            {
                _errorOutput.WriteLine("Error: Failed update language file");
                return false;
            }
            _configDataTranslateJsonNodes[language] = updatedTranslationJsonNode;
            transation.Commit();
            return true;
        }

        public bool RemoveLanguage(string language, bool deleteFile)
        {
            using var trasactionGroup = new TransactionGroup(2);
            if (deleteFile)
            {
                string translationJsonPath = _fileSourceSettings.DatabaseTranslateFilePath(language);
                if (File.Exists(translationJsonPath))
                {
                    trasactionGroup.Add(DeleteFileTransaction.Create(translationJsonPath));
                }
            }
            var updatedJsonNode = _configDataJsonNode;
            if (updatedJsonNode != null && updatedJsonNode.Languages.Contains(language))
            {
                var builder = new ConfigDataJsonNode.Builder(updatedJsonNode);
                builder.Languages.Remove(language);
                updatedJsonNode = builder.Build();
                trasactionGroup.Add(ConfigSaveTransaction.Create(updatedJsonNode, _fileSourceSettings.DatabaseFilePath));
            }
            if (!trasactionGroup.Apply())
            {
                _errorOutput.WriteLine("Error: Failed remove language");
                return false;
            }
            _configDataTranslateJsonNodes.Remove(language);
            _configDataJsonNode = updatedJsonNode;
            trasactionGroup.Commit();
            return true;
        }

        public ISet<string>? GetSupportedLanguages()
        {
            if (_configDataJsonNode == null)
            {
                _errorOutput.WriteLine("Error: Database not loaded");
                return null;
            }
            return _configDataJsonNode.Languages;
        }
    }
}
