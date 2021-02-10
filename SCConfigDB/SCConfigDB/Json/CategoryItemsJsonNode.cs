using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class CategoryItemsJsonNode<T> where T : KeyedItemJsonNode
    {
        [JsonProperty("categories", Required = Required.Always, Order = 0)]
        public CategoryJsonNode[] Categories { get; private set; } = new CategoryJsonNode[0];
        [JsonProperty("items", Required = Required.Always, Order = 1)]
        public T[] Items { get; private set; } = new T[0];

        [JsonConstructor]
        public CategoryItemsJsonNode() { }

        protected CategoryItemsJsonNode(CategoryJsonNode[] categories, T[] items)
        {
            Categories = categories;
            Items = items;
        }

        protected CategoryItemsJsonNode(Builder builder)
        {
            if (builder.Categories.Count != 0)
            {
                Categories = builder.Categories.ToArray();
            }
            if (builder.Items.Count != 0)
            {
                Items = builder.Items.ToArray();
            }
        }

        public CategoryJsonNode? GetCategory(string key) => Categories.FirstOrDefault(c => c.IsKeyEqual(key));

        public int GetCategoryIndex(string key) => Array.FindIndex(Categories, c => c.IsKeyEqual(key));

        public T? GetItem(string key) => Items.FirstOrDefault(c => c.IsKeyEqual(key));

        public int GetItemIndex(string key) => Array.FindIndex(Items, c => c.IsKeyEqual(key));

        public class Builder
        {
            public List<CategoryJsonNode> Categories { get; } = new List<CategoryJsonNode>();
            public List<T> Items { get; } = new List<T>();

            public CategoryItemsJsonNode<T> Build() => new CategoryItemsJsonNode<T>(this);
        }
    }
}
