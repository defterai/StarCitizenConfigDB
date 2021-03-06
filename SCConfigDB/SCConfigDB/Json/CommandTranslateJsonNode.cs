using System.Collections.Generic;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class CommandTranslateJsonNode : KeyedItemJsonNode
    {
        [JsonProperty("name", Required = Required.Always, Order = 1)]
        public string Name { get; }
        [JsonProperty("desc", Order = 2)]
        public string? Description { get; private set; }
        [JsonProperty("params", Order = 3)]
        public ParamTranslateJsonNode[]? Parameters { get; private set; }

        [JsonConstructor]
        public CommandTranslateJsonNode(string key, string name) : base(key)
        {
            Name = name;
        }

        private CommandTranslateJsonNode(Builder builder) : this(builder.Key, builder.Name)
        {
            Description = builder.Description;
            if (builder.Parameters.Count != 0)
            {
                Parameters = builder.Parameters.ToArray();
            }
        }

        public sealed new class Builder : KeyedItemJsonNode.Builder
        {
            public string Name { get; }
            public string? Description { get; set; }
            public List<ParamTranslateJsonNode> Parameters { get; } = new List<ParamTranslateJsonNode>();

            public Builder(string key, string name) : base(key)
            {
                Name = name;
            }

            public new CommandTranslateJsonNode Build() => new CommandTranslateJsonNode(this);
        }
    }
}
