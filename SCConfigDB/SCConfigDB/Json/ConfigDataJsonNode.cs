using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ConfigDataJsonNode
    {
        [JsonProperty("commands", Required = Required.Always)]
        public CommandsJsonNode Commands { get; }
        [JsonProperty("settings", Required = Required.Always)]
        public SettingsJsonNode Settings { get; }

        [JsonConstructor]
        public ConfigDataJsonNode(CommandsJsonNode commands, SettingsJsonNode settings)
        {
            Commands = commands;
            Settings = settings;
        }

        public ConfigDataJsonNode(ConfigDataJsonNode node, ConfigDataTranslateJsonNode translateNode)
        {
            Commands = node.Commands.TranslateWith(translateNode.Commands);
            Settings = node.Settings.TranslateWith(translateNode.Settings);
        }

        public ConfigDataJsonNode TranslateWith(ConfigDataTranslateJsonNode translateNode) =>
            new ConfigDataJsonNode(this, translateNode);
    }
}
