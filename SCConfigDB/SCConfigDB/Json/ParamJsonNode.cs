using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ParamJsonNode
    {
        [JsonProperty("name", Required = Required.Always, Order = 0)]
        public string Name { get; }
        [JsonProperty("desc", Order = 1)]
        public string? Description { get; }
        [JsonProperty("values", Required = Required.Always, Order = 2)]
        public ValuesJsonNode Values { get; }

        [JsonConstructor]
        public ParamJsonNode(string name, ValuesJsonNode values)
        {
            Name = name;
            Values = values;
        }
        private ParamJsonNode(Builder builder) : this(builder.Name, builder.Values)
        {
            Description = builder.Description;
        }

        private ParamJsonNode(ParamJsonNode node, ParamTranslateJsonNode translateNode)
        {
            Name = translateNode.Name;
            Description = translateNode.Description ?? node.Description;
            Values = node.Values.TranslateWith(translateNode.Values);
        }

        public ParamJsonNode TranslateWith(ParamTranslateJsonNode translateNode) => new ParamJsonNode(this, translateNode);

        public sealed class Builder
        {
            public string Name { get; }
            public string? Description { get; set; }
            public ValuesJsonNode Values { get; }

            public Builder(string name, ValuesJsonNode values)
            {
                Name = name;
                Values = values;
            }

            public ParamJsonNode Build() => new ParamJsonNode(this);
        }
    }
}
