using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class CommandTranslateJsonNode : KeyedItemJsonNode
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }
        [JsonProperty("desc")]
        public string? Description { get; }
        [JsonProperty("params")]
        public ParamTranslateJsonNode[]? Parameters { get; internal set; }

        [JsonConstructor]
        public CommandTranslateJsonNode(string key, string name) : base(key)
        {
            Name = name;
        }
    }
}
