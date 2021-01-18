using System.Collections.Generic;
using System.Linq;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class FloatSetting : BaseSetting
    {
        public float DefaultValue { get; }
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

        public sealed class Factory : IFactory
        {
            public BaseSetting Build(SettingJsonNode node) => new FloatSetting(node);
        }

        public sealed class Builder : BaseBuilder
        {
            public float DefaultValue { get; set; }
            public Dictionary<float, string> Values { get; } = new Dictionary<float, string>();
            public bool Range { get; set; }

            public Builder(string key, string name) : base(key, name) { }

            public override BaseSetting Build() => new FloatSetting(this);
        }
    }
}
