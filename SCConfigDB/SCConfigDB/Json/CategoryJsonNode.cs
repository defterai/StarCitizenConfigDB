using System;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class CategoryJsonNode : KeyedItemJsonNode
    {
        [JsonProperty("name", Required = Required.Always, Order = 1)]
        public string Name { get; }

        [JsonConstructor]
        public CategoryJsonNode(string key, string name) : base(key)
        {
            Name = name;
        }

        private CategoryJsonNode(Builder builder) : this(builder.Key, builder.Name) { }

        private CategoryJsonNode(CategoryJsonNode node, CategoryJsonNode translateNode) : base(node.Key)
        {
            if (!IsKeyEqual(translateNode.Key))
            {
                throw new InvalidOperationException($"translate node key {translateNode.Key} is not equal to {node.Key}");
            }
            Name = translateNode.Name;
        }

        public CategoryJsonNode TranslateWith(CategoryJsonNode translateNode) =>
            new CategoryJsonNode(this, translateNode);

        public new sealed class Builder : KeyedItemJsonNode.Builder
        {
            public string Name { get; }

            public Builder(string key, string name) : base(key)
            {
                Name = name;
            }

            public new CategoryJsonNode Build() => new CategoryJsonNode(this);
        } 
    }
}
