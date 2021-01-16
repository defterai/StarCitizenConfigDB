using System;
using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Collection;
using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public class ValueJsonNode
    {
        [JsonProperty("value", Required = Required.Always)]
        public string Value { get; }
        [JsonProperty("name")]
        public string Name { get; }

        [JsonConstructor]
        public ValueJsonNode(string value, string? name)
        {
            Value = value;
            Name = name ?? value;
        }

        public ValueJsonNode TranslateWith(ValueJsonNode translateNode) => new ValueJsonNode(Value, translateNode.Name);

        public int IntegerValue() => int.Parse(Value);

        public float FloatValue() => float.Parse(Value);

        public static IReadOnlyDictionary<T, string> LoadValues<T>(ValueJsonNode[]? valuesList, Func<ValueJsonNode, T> valueParser)
        {
            if (valuesList != null && valuesList.Length != 0)
            {
                var values = new OrderedDictionary<T, string>(valuesList.Length);
                foreach (var valueNode in valuesList)
                {
                    values.Add(valueParser(valueNode), valueNode.Name);
                }
                return values;
            }
            return OrderedDictionary<T, string>.Empty;
        }
    }
}
