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

        public sealed class Factory : IFactory
        {
            public BaseSetting Build(SettingJsonNode node) => new IntegerSetting(node);
        }
    }
}
