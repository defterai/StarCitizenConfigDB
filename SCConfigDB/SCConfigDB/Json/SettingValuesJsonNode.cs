using System;
using System.Linq;
using Newtonsoft.Json;
using Defter.StarCitizen.ConfigDB.Collection;
using System.Collections.Generic;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class SettingValuesJsonNode
    {
        [JsonProperty("type", Required = Required.Always, Order = 0)]
        public ValueJsonType Type { get; }
        [JsonProperty("default", Order = 1)]
        public string? DefaultValue { get; private set; }
        [JsonProperty("list", Order = 2)]
        public ValueJsonNode[]? List { get; private set; }

        [JsonConstructor]
        public SettingValuesJsonNode(ValueJsonType type)
        {
            Type = type;
        }

        private SettingValuesJsonNode(Builder builder) : this(builder.Type)
        {
            DefaultValue = builder.DefaultValue;
            if (builder.ValueList.Count != 0)
            {
                List = builder.ValueList.ToArray();
            }
        }

        private SettingValuesJsonNode(SettingValuesJsonNode node, ValueJsonNode[]? translateNodes)
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

        public SettingValuesJsonNode TranslateWith(ValueJsonNode[]? translateNodes) => new SettingValuesJsonNode(this, translateNodes);

        public ValueJsonNode? GetValueNode(string value) => List.FirstOrDefault(v => string.Equals(v.Value, value));

        public int GetValueIndex(string value) => Array.FindIndex(List, v => string.Equals(v.Value, value));

        public bool? BooleanDefault()
        {
            if (DefaultValue != null)
                return bool.Parse(DefaultValue);
            return null;
        }

        public int? IntegerDefault()
        {
            if (DefaultValue != null)
                return int.Parse(DefaultValue);
            return null;
        }

        public float? FloatDefault()
        {
            if (DefaultValue != null)
                return float.Parse(DefaultValue);
            return null;
        }

        public sealed class Builder
        {
            public ValueJsonType Type { get; }
            public string? DefaultValue { get; set; }
            public List<ValueJsonNode> ValueList { get; } = new List<ValueJsonNode>();

            public Builder(ValueJsonType type)
            {
                Type = type;
            }

            public SettingValuesJsonNode Build() => new SettingValuesJsonNode(this);
        }
    }
}
