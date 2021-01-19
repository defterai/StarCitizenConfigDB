using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ConfigDataJsonNode
    {
        [JsonProperty("$schema", Order = 0)]
        public string? Schema { get; }
        [JsonProperty("languages", Order = 1)]
        public ISet<string> Languages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        [JsonProperty("commands", Required = Required.Always, Order = 2)]
        public CommandsJsonNode Commands { get; }
        [JsonProperty("settings", Required = Required.Always, Order = 3)]
        public SettingsJsonNode Settings { get; }

        [JsonConstructor]
        public ConfigDataJsonNode(CommandsJsonNode commands, SettingsJsonNode settings)
        {
            Commands = commands;
            Settings = settings;
        }

        private ConfigDataJsonNode(Builder builder) : this(builder.Commands, builder.Settings)
        {
            Schema = builder.Schema;
            Languages = builder.Languages;
        }

        private ConfigDataJsonNode(ConfigDataJsonNode node, ConfigDataTranslateJsonNode translateNode)
        {
            Schema = node.Schema;
            Languages = node.Languages;
            Commands = node.Commands.TranslateWith(translateNode.Commands);
            Settings = node.Settings.TranslateWith(translateNode.Settings);
        }

        public ConfigDataJsonNode TranslateWith(ConfigDataTranslateJsonNode translateNode) =>
            new ConfigDataJsonNode(this, translateNode);

        public sealed class Builder
        {
            public string? Schema { get; set; }
            public ISet<string> Languages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            public CommandsJsonNode Commands { get; }
            public SettingsJsonNode Settings { get; }

            public Builder(CommandsJsonNode commands, SettingsJsonNode settings)
            {
                Commands = commands;
                Settings = settings;
            }

            public ConfigDataJsonNode Build() => new ConfigDataJsonNode(this);
        }
    }
}
