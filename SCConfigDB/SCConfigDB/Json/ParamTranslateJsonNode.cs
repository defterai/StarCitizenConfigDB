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
    }
}
