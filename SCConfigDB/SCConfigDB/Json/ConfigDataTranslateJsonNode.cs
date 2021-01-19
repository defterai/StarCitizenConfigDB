using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ConfigDataTranslateJsonNode
    {
        [JsonProperty("$schema", Order = 0)]
        public string? Schema { get; private set; }
        [JsonProperty("commands", Required = Required.Always, Order = 1)]
        public CommandsTranslateJsonNode Commands { get; }
        [JsonProperty("settings", Required = Required.Always, Order = 2)]
        public SettingsTranslateJsonNode Settings { get; }

        [JsonConstructor]
        public ConfigDataTranslateJsonNode(CommandsTranslateJsonNode commands, SettingsTranslateJsonNode settings)
        {
            Commands = commands;
            Settings = settings;
        }

        private ConfigDataTranslateJsonNode(Builder builder) : this(builder.Commands, builder.Settings)
        {
            Schema = builder.Schema;
        }

        public sealed class Builder
        {
            public string? Schema { get; set; }
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
