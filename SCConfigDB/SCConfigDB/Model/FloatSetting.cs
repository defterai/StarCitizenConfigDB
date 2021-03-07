using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Defter.StarCitizen.ConfigDB.Json;
using Defter.StarCitizen.ConfigDB.Collection;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class FloatSetting : BaseSetting
    {
        public float? DefaultValue { get; }
        public float? Step { get; }
        public IReadOnlyDictionary<float, string?> Values { get; }
        public bool Range { get; }
        public float MinValue => Values.Keys.Min();
        public float MaxValue => Values.Keys.Max();
        public IEnumerable<KeyValuePair<float, string>> LabeledValues => Values.ToNonNullableKeyValues();

        private FloatSetting(SettingJsonNode node) : base(node)
        {
            DefaultValue = node.Values.FloatDefault();
            Range = node.Values.Type == ValueJsonType.RangeFloat;
            Step = Range ? node.Values.FloatStep() : null;
            Values = ValueJsonNode.LoadValues(node.Values.List, n => n.FloatValue());
        }

        private FloatSetting(Builder builder) : base(builder)
        {
            DefaultValue = builder.DefaultValue;
            Step = builder.Range ? builder.Step : null;
            Values = builder.Values;
            Range = builder.Range;
        }

        public override void ExctractValueNodes(List<ValueJsonNode> nodes) =>
            ValueJsonNode.ValuesToNodes(Values, nodes, v => v.ToString(CultureInfo.InvariantCulture));

        public override SettingValuesJsonNode GetValuesNode()
        {
            var builder = new SettingValuesJsonNode.Builder(Range ? ValueJsonType.RangeFloat : ValueJsonType.Float);
            if (DefaultValue.HasValue)
            {
                builder.DefaultValue = DefaultValue.Value.ToString(CultureInfo.InvariantCulture);
            }
            if (Range && Step.HasValue)
            {
                builder.Step = Step.Value.ToString(CultureInfo.InvariantCulture);
            }
            foreach (var valuePair in Values)
            {
                var valueBuilder = new ValueJsonNode.Builder(valuePair.Key.ToString(CultureInfo.InvariantCulture))
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
            public float? Step { get; set; }
            public Dictionary<float, string?> Values { get; } = new Dictionary<float, string?>();
            public bool Range { get; set; }

            public Builder(string key, string name) : base(key, name) { }

            public override BaseSetting Build() => new FloatSetting(this);
        }
    }
}
