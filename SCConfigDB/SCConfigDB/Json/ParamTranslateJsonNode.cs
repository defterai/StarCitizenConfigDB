using System.Collections.Generic;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ParamTranslateJsonNode
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }
        [JsonProperty("desc")]
        public string? Description { get; }
        [JsonProperty("values")]
        public ValueJsonNode[]? Values { get; internal set; }

        [JsonConstructor]
        public ParamTranslateJsonNode(string name)
        {
            Name = name;
        }

        private ParamTranslateJsonNode(Builder builder) : this(builder.Name)
        {
            Description = builder.Description;
            if (builder.Values.Count != 0)
            {
                Values = builder.Values.ToArray();
            }
        }

        public sealed class Builder
        {
            public string Name { get; }
            public string? Description { get; set; }
            public List<ValueJsonNode> Values { get; } = new List<ValueJsonNode>();

            public Builder(string name)
            {
                Name = name;
            }

            public ParamTranslateJsonNode Build() => new ParamTranslateJsonNode(this);
        }
    }
}
