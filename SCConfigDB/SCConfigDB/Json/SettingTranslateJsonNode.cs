using System.Collections.Generic;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class SettingTranslateJsonNode : KeyedItemJsonNode
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }
        [JsonProperty("desc")]
        public string? Description { get; }
        [JsonProperty("values")]
        public ValueJsonNode[]? Values { get; internal set; }

        [JsonConstructor]
        public SettingTranslateJsonNode(string key, string name) : base(key)
        {
            Name = name;
        }

        private SettingTranslateJsonNode(Builder builder) : this(builder.Key, builder.Name)
        {
            Description = builder.Description;
            if (builder.Values.Count != 0)
            {
                Values = builder.Values.ToArray();
            }
        }

        public new sealed class Builder : KeyedItemJsonNode.Builder
        {
            public string Name { get; }
            public string? Description { get; set; }
            public List<ValueJsonNode> Values { get; } = new List<ValueJsonNode>();

            public Builder(string key, string name) : base(key)
            {
                Name = name;
            }

            public new SettingTranslateJsonNode Build() => new SettingTranslateJsonNode(this);
        }
    }
}
