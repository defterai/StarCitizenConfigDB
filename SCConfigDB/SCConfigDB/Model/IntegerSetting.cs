using System.Collections.Generic;
using System.Linq;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class IntegerSetting : BaseSetting
    {
        public int DefaultValue { get; }
        public IReadOnlyDictionary<int, string> Values { get; }
        public bool Range { get; }
        public int MinValue => Values.Keys.Min();
        public int MaxValue => Values.Keys.Max();

        private IntegerSetting(SettingJsonNode node) : base(node)
        {
            DefaultValue = node.Values.IntegerDefault();
            Values = ValueJsonNode.LoadValues(node.Values.List, n => n.IntegerValue());
            Range = node.Values.Type == ValueJsonType.RangeInt;
        }

        private IntegerSetting(Builder builder) : base(builder)
        {
            DefaultValue = builder.DefaultValue;
            Values = builder.Values;
            Range = builder.Range;
        }

        public sealed class Factory : IFactory
        {
            public BaseSetting Build(SettingJsonNode node) => new IntegerSetting(node);
        }

        public sealed class Builder : BaseBuilder
        {
            public int DefaultValue { get; set; }
            public Dictionary<int, string> Values { get; } = new Dictionary<int, string>();
            public bool Range { get; set; }

            public Builder(string key, string name) : base(key, name) { }

            public override BaseSetting Build() => new IntegerSetting(this);
        }
    }
}
