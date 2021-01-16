using System;
using System.Linq;
using Newtonsoft.Json;
using Defter.StarCitizen.ConfigDB.Collection;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class ValuesJsonNode
    {
        [JsonProperty("type", Required = Required.Always)]
        public ValueJsonType Type { get; }
        [JsonProperty("default", Required = Required.Always)]
        public string DefaultValue { get; }
        [JsonProperty("list")]
        public ValueJsonNode[]? List { get; internal set; }

        [JsonConstructor]
        public ValuesJsonNode(ValueJsonType type, string defaultValue)
        {
            Type = type;
            DefaultValue = defaultValue;
        }

        public ValuesJsonNode(ValuesJsonNode node, ValueJsonNode[]? translateNodes)
        {
            Type = node.Type;
            DefaultValue = node.DefaultValue;
            if (node.List != null)
            {
                List = ArrayHelper.Clone(node.List);
                if (translateNodes != null)
                {
                    foreach (var translateNode in translateNodes)
                    {
                        int? index = node.GetValueIndex(translateNode.Value);
                        if (index.HasValue)
                        {
                            List[index.Value] = node.List[index.Value].TranslateWith(translateNode);
                        }
                    }
                }
            }
        }

        public ValuesJsonNode TranslateWith(ValueJsonNode[]? translateNodes) => new ValuesJsonNode(this, translateNodes);

        public ValueJsonNode? GetValueNode(string value) => List.FirstOrDefault(v => string.Equals(v.Value, value));

        public int? GetValueIndex(string value) => Array.FindIndex(List, v => string.Equals(v.Value, value));

        public bool BooleanDefault() => bool.Parse(DefaultValue);

        public int IntegerDefault() => int.Parse(DefaultValue);

        public float FloatDefault() => float.Parse(DefaultValue);
    }
}
