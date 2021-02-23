using System.Collections.Generic;
using System.Linq;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class IntegerSetting : BaseSetting
    {
        public int? DefaultValue { get; }
        public int? Step { get; }
        public IReadOnlyDictionary<int, string> Values { get; }
        public bool Range { get; }
        public int MinValue => Values.Keys.Min();
        public int MaxValue => Values.Keys.Max();

        private IntegerSetting(SettingJsonNode node) : base(node)
        {
            DefaultValue = node.Values.IntegerDefault();
            Range = node.Values.Type == ValueJsonType.RangeInt;
            Step = Range ? node.Values.IntegerStep() : null;
            Values = ValueJsonNode.LoadValues(node.Values.List, n => n.IntegerValue());
        }

        private IntegerSetting(Builder builder) : base(builder)
        {
            DefaultValue = builder.DefaultValue;
            Step = builder.Range ? builder.Step : null;
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
            if (Range && Step.HasValue)
            {
                builder.Step = Step.Value.ToString();
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
            public BaseSetting Build(SettingJsonNode node) => new IntegerSetting(node);
        }

        public new sealed class Builder : BaseSetting.Builder
        {
            public int? DefaultValue { get; set; }
            public int? Step { get; set; }
            public Dictionary<int, string> Values { get; } = new Dictionary<int, string>();
            public bool Range { get; set; }

            public Builder(string key, string name) : base(key, name) { }

            public override BaseSetting Build() => new IntegerSetting(this);
        }
    }
}
