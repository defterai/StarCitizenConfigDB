using System.Collections.Generic;
using System.Linq;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class FloatSetting : BaseSetting
    {
        public float? DefaultValue { get; }
        public IReadOnlyDictionary<float, string> Values { get; }
        public bool Range { get; }
        public float MinValue => Values.Keys.Min();
        public float MaxValue => Values.Keys.Max();

        private FloatSetting(SettingJsonNode node) : base(node)
        {
            DefaultValue = node.Values.FloatDefault();
            Values = ValueJsonNode.LoadValues(node.Values.List, n => n.FloatValue());
            Range = node.Values.Type == ValueJsonType.RangeFloat;
        }

        private FloatSetting(Builder builder) : base(builder)
        {
            DefaultValue = builder.DefaultValue;
            Values = builder.Values;
            Range = builder.Range;
        }

        public override void ExctractValueNodes(List<ValueJsonNode> nodes) =>
            ValueJsonNode.ValuesToNodes(Values, nodes, v => v.ToString());

        public override SettingValuesJsonNode GetValuesNode()
        {
            var builder = new SettingValuesJsonNode.Builder(Range ? ValueJsonType.RangeInt : ValueJsonType.Int);
            if (DefaultValue.HasValue)
            {
                builder.DefaultValue = DefaultValue.Value.ToString();
            }
            foreach (var valuePair in Values)
            {
                var valueBuilder = new ValueJsonNode.Builder(valuePair.Key.ToString())
                {
                    Name = valuePair.Value
                };
                builder.ValueList.Add(valueBuilder.Build());
            }
            return builder.Build();
        }

        public sealed class Factory : IFactory
        {
            public BaseSetting Build(SettingJsonNode node) => new FloatSetting(node);
        }

        public new sealed class Builder : BaseSetting.Builder
        {
            public float? DefaultValue { get; set; }
            public Dictionary<float, string> Values { get; } = new Dictionary<float, string>();
            public bool Range { get; set; }

            public Builder(string key, string name) : base(key, name) { }

            public override BaseSetting Build() => new FloatSetting(this);
        }
    }
}
