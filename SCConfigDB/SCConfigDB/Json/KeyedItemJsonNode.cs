using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class KeyedItemJsonNode
    {
        [JsonProperty("key", Required = Required.Always)]
        public string Key { get; }

        [JsonConstructor]
        public KeyedItemJsonNode(string key)
        {
            Key = key;
        }

        public bool IsKeyEqual(string key) => string.Equals(Key, key);

        public class Builder
        {
            public string Key { get; }

            public Builder(string key)
            {
                Key = key;
            }

            public KeyedItemJsonNode Build() => new KeyedItemJsonNode(Key);
        }
    }
}
