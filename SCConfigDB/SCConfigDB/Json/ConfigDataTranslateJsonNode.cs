using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ConfigDataTranslateJsonNode
    {
        [JsonProperty("commands", Required = Required.Always)]
        public CommandsTranslateJsonNode Commands { get; }
        [JsonProperty("settings", Required = Required.Always)]
        public SettingsTranslateJsonNode Settings { get; }

        [JsonConstructor]
        public ConfigDataTranslateJsonNode(CommandsTranslateJsonNode commands, SettingsTranslateJsonNode settings)
        {
            Commands = commands;
            Settings = settings;
        }
    }
}
