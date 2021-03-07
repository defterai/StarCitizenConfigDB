using System;
using Newtonsoft.Json;
using Defter.StarCitizen.ConfigDB.Collection;
using System.Collections.Generic;
using System.Globalization;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class SettingValuesJsonNode
    {
        [JsonProperty("type", Required = Required.Always, Order = 0)]
        public ValueJsonType Type { get; }
        [JsonProperty("default", Order = 1)]
        public string? DefaultValue { get; private set; }
        [JsonProperty("step", Order = 2)]
        public string? Step { get; private set; }
        [JsonProperty("list", Order = 3)]
        public ValueJsonNode[]? List { get; private set; }

        [JsonConstructor]
        public SettingValuesJsonNode(ValueJsonType type)
        {
            Type = type;
        }

        private SettingValuesJsonNode(Builder builder) : this(builder.Type)
        {
            DefaultValue = builder.DefaultValue;
            Step = builder.Step;
            if (builder.ValueList.Count != 0)
            {
                List = builder.ValueList.ToArray();
            }
        }

        private SettingValuesJsonNode(SettingValuesJsonNode node, ValueJsonNode[]? translateNodes)
        {
            Type = node.Type;
            DefaultValue = node.DefaultValue;
            Step = node.Step;
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

        public ValueJsonNode? GetValueNode(string value)
        {
            if (List != null)
            {
                int index = GetValueIndex(Type, List, value);
                return index != -1 ? List[index] : null;
            }
            return null;
        }

        public int GetValueIndex(string value) => List != null ? GetValueIndex(Type, List, value) : -1;

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
                return float.Parse(DefaultValue, CultureInfo.InvariantCulture);
            return null;
        }

        public int? IntegerStep()
        {
            if (Step != null)
                return int.Parse(Step);
            return null;
        }

        public float? FloatStep()
        {
            if (Step != null)
                return float.Parse(Step, CultureInfo.InvariantCulture);
            return null;
        }

        private static int GetValueIndex(ValueJsonType type, ValueJsonNode[] valuesNodes, string value)
        {
            switch (type)
            {
                case ValueJsonType.Bool:
                    return -1;
                case ValueJsonType.Int:
                case ValueJsonType.RangeInt:
                    int searchIntValue = int.Parse(value);
                    return Array.FindIndex(valuesNodes, n => n.IntegerValue() == searchIntValue);
                case ValueJsonType.Float:
                case ValueJsonType.RangeFloat:
                    float searchFloatValue = float.Parse(value, CultureInfo.InvariantCulture);
                    return Array.FindIndex(valuesNodes, n => n.FloatValue() == searchFloatValue);
                case ValueJsonType.String:
                default:
                    return Array.FindIndex(valuesNodes, n => string.Equals(n.Value, value));
            }
        }

        public sealed class Builder
        {
            public ValueJsonType Type { get; }
            public string? DefaultValue { get; set; }
            public string? Step { get; set; }
            public List<ValueJsonNode> ValueList { get; } = new List<ValueJsonNode>();

            public Builder(ValueJsonType type)
            {
                Type = type;
            }

            public SettingValuesJsonNode Build() => new SettingValuesJsonNode(this);
        }
    }
}
