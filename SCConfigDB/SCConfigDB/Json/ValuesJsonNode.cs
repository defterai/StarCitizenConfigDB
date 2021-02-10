using System;
using System.Linq;
using Newtonsoft.Json;
using Defter.StarCitizen.ConfigDB.Collection;
using System.Collections.Generic;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class ValuesJsonNode
    {
        [JsonProperty("type", Required = Required.Always, Order = 0)]
        public ValueJsonType Type { get; }
        [JsonProperty("default", Required = Required.Always, Order = 1)]
        public string DefaultValue { get; }
        [JsonProperty("list", Order = 2)]
        public ValueJsonNode[]? List { get; private set; }

        [JsonConstructor]
        public ValuesJsonNode(ValueJsonType type, string defaultValue)
        {
            Type = type;
            DefaultValue = defaultValue;
        }

        private ValuesJsonNode(Builder builder) : this(builder.Type, builder.DefaultValue)
        {
            if (builder.ValueList.Count != 0)
            {
                List = builder.ValueList.ToArray();
            }
        }

        private ValuesJsonNode(ValuesJsonNode node, ValueJsonNode[]? translateNodes)
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
                        int index = node.GetValueIndex(translateNode.Value);
                        if (index != -1)
                        {
                            List[index] = node.List[index].TranslateWith(translateNode);
                        }
                    }
                }
            }
        }

        public ValuesJsonNode TranslateWith(ValueJsonNode[]? translateNodes) => new ValuesJsonNode(this, translateNodes);

        public ValueJsonNode? GetValueNode(string value) => List.FirstOrDefault(v => string.Equals(v.Value, value));

        public int GetValueIndex(string value) => Array.FindIndex(List, v => string.Equals(v.Value, value));

        public bool BooleanDefault() => bool.Parse(DefaultValue);

        public int IntegerDefault() => int.Parse(DefaultValue);

        public float FloatDefault() => float.Parse(DefaultValue);

        public sealed class Builder
        {
            public ValueJsonType Type { get; }
            public string DefaultValue { get; }
            public List<ValueJsonNode> ValueList { get; } = new List<ValueJsonNode>();

            public Builder(ValueJsonType type, string defaultValue)
            {
                Type = type;
                DefaultValue = defaultValue;
            }

            public ValuesJsonNode Build() => new ValuesJsonNode(this);
        }
    }
}
