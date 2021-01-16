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

        public sealed class Factory : IFactory
        {
            public BaseSetting Build(SettingJsonNode node) => new FloatSetting(node);
        }
    }
}
