using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ConfigDataTranslateJsonNode
    {
        [JsonProperty("commands", Required = Required.Always, Order = 0)]
        public CommandsTranslateJsonNode Commands { get; }
        [JsonProperty("settings", Required = Required.Always, Order = 1)]
        public SettingsTranslateJsonNode Settings { get; }

        [JsonConstructor]
        public ConfigDataTranslateJsonNode(CommandsTranslateJsonNode commands, SettingsTranslateJsonNode settings)
        {
            Commands = commands;
            Settings = settings;
        }

        private ConfigDataTranslateJsonNode(Builder builder) : this(builder.Commands, builder.Settings) { }

        public sealed class Builder
        {
            public CommandsTranslateJsonNode Commands { get; }
            public SettingsTranslateJsonNode Settings { get; }

            public Builder(CommandsTranslateJsonNode commands, SettingsTranslateJsonNode settings)
            {
                Commands = commands;
                Settings = settings;
            }

            public ConfigDataTranslateJsonNode Build() => new ConfigDataTranslateJsonNode(this);
        }
    }
}
