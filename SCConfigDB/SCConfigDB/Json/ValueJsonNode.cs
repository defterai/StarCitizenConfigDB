using System;
using System.Collections.Generic;
using System.Globalization;
using Defter.StarCitizen.ConfigDB.Collection;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class ValueJsonNode
    {
        [JsonProperty("value", Required = Required.Always, Order = 0)]
        public string Value { get; }
        [JsonProperty("name", Order = 1)]
        public string? Name { get; }

        [JsonConstructor]
        public ValueJsonNode(string value, string? name)
        {
            Value = value;
            Name = name;
        }

        private ValueJsonNode(Builder builder) : this(builder.Value, builder.Name) { }

        public ValueJsonNode TranslateWith(ValueJsonNode translateNode) => new ValueJsonNode(Value, translateNode.Name ?? Name);

        public int IntegerValue() => int.Parse(Value);

        public float FloatValue() => float.Parse(Value, CultureInfo.InvariantCulture);

        public static IReadOnlyDictionary<T, string?> LoadValues<T>(ValueJsonNode[]? valuesList, Func<ValueJsonNode, T> valueParser)
        {
            if (valuesList != null && valuesList.Length != 0)
            {
                var values = new OrderedDictionary<T, string?>(valuesList.Length);
                foreach (var valueNode in valuesList)
                {
                    values.Add(valueParser(valueNode), valueNode.Name);
                }
                return values;
            }
            return OrderedDictionary<T, string?>.Empty;
        }

        public static void ValuesToNodes<T>(IReadOnlyDictionary<T, string?> values, List<ValueJsonNode> nodes, Func<T, string> valueFormatter)
        {
            foreach (var valuePair in values)
            {
                nodes.Add(new ValueJsonNode(valueFormatter(valuePair.Key), valuePair.Value));
            }
        }

        public sealed class Builder
        {
            public string Value { get; }
            public string? Name { get; set; }

            public Builder(string value)
            {
                Value = value;
            }

            public ValueJsonNode Build() => new ValueJsonNode(this);
        }
    }
}
