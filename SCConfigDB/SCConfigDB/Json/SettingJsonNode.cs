using System;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class SettingJsonNode : KeyedItemJsonNode
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }
        [JsonProperty("category", Required = Required.Always)]
        public string Category { get; }
        [JsonProperty("desc")]
        public string? Description { get; }
        [JsonProperty("values", Required = Required.Always)]
        public ValuesJsonNode Values { get; }

        [JsonConstructor]
        public SettingJsonNode(string key, string name, string category, ValuesJsonNode values) : base(key)
        {
            Name = name;
            Category = category;
            Values = values;
        }

        public SettingJsonNode(SettingJsonNode node, SettingTranslateJsonNode translateNode)
            : base(node.Key)
        {
            if (!node.IsKeyEqual(translateNode.Key))
            {
                throw new InvalidOperationException($"translate node key {translateNode.Key} is not equal to {node.Key}");
            }
            if ((node.Values.List == null || node.Values.List.Length == 0) && translateNode.Values != null)
            {
                throw new InvalidOperationException($"translate node {translateNode.Key} should not contains values");
            }
            Name = translateNode.Name;
            Category = node.Category;
            Description = translateNode.Description ?? node.Description;
            Values = node.Values.TranslateWith(translateNode.Values);
        }

        public SettingJsonNode TranslateWith(SettingTranslateJsonNode translateNode) =>
            new SettingJsonNode(this, translateNode);
    }
}
