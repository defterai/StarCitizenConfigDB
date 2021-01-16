using System;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class CategoryJsonNode : KeyedItemJsonNode
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }

        [JsonConstructor]
        public CategoryJsonNode(string key, string name) : base(key)
        {
            Name = name;
        }

        public CategoryJsonNode(CategoryJsonNode node, CategoryJsonNode translateNode) : base(node.Key)
        {
            if (!IsKeyEqual(translateNode.Key))
            {
                throw new InvalidOperationException($"translate node key {translateNode.Key} is not equal to {node.Key}");
            }
            Name = translateNode.Name;
        }

        public CategoryJsonNode TranslateWith(CategoryJsonNode translateNode) =>
            new CategoryJsonNode(this, translateNode);
    }
}
