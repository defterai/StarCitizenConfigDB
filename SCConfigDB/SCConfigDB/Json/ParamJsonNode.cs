using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ParamJsonNode
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }
        [JsonProperty("desc")]
        public string? Description { get; }
        [JsonProperty("values", Required = Required.Always)]
        public ValuesJsonNode Values { get; }

        [JsonConstructor]
        public ParamJsonNode(string name, ValuesJsonNode values)
        {
            Name = name;
            Values = values;
        }

        public ParamJsonNode(ParamJsonNode node, ParamTranslateJsonNode translateNode)
        {
            Name = translateNode.Name;
            Description = translateNode.Description ?? node.Description;
            Values = node.Values.TranslateWith(translateNode.Values);
        }

        public ParamJsonNode TranslateWith(ParamTranslateJsonNode translateNode) => new ParamJsonNode(this, translateNode);
    }
}
