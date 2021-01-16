using System;
using System.Linq;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class CategoryItemsJsonNode<T> where T : KeyedItemJsonNode
    {
        [JsonProperty("categories", Required = Required.Always)]
        public CategoryJsonNode[] Categories { get; private set; } = new CategoryJsonNode[0];
        [JsonProperty("items", Required = Required.Always)]
        public T[] Items { get; private set; } = new T[0];

        [JsonConstructor]
        public CategoryItemsJsonNode() { }

        protected CategoryItemsJsonNode(CategoryJsonNode[] categories, T[] items)
        {
            Categories = categories;
            Items = items;
        }

        public CategoryJsonNode? GetCategory(string key) => Categories.FirstOrDefault(c => c.IsKeyEqual(key));

        public int? GetCategoryIndex(string key) => Array.FindIndex(Categories, c => c.IsKeyEqual(key));

        public T? GetItem(string key) => Items.FirstOrDefault(c => c.IsKeyEqual(key));

        public int? GetItemIndex(string key) => Array.FindIndex(Items, c => c.IsKeyEqual(key));
    }
}
