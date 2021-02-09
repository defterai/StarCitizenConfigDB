using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class BooleanSetting : BaseSetting
    {
        public bool? DefaultValue { get; }

        private BooleanSetting(SettingJsonNode node) : base(node)
        {
            DefaultValue = node.Values.BooleanDefault();
        }

        private BooleanSetting(Builder builder) : base(builder)
        {
            DefaultValue = builder.DefaultValue;
        }

        public override void ExctractValueNodes(List<ValueJsonNode> nodes) { }

        public override SettingValuesJsonNode GetValuesNode()
        {
            var builder = new SettingValuesJsonNode.Builder(ValueJsonType.Bool);
            if (DefaultValue.HasValue)
            {
                builder.DefaultValue = DefaultValue.Value.ToString().ToLower();
            }
            return builder.Build();
        }

        public sealed class Factory : IFactory
        {
            public BaseSetting Build(SettingJsonNode node) => new BooleanSetting(node);
        }

        public new sealed class Builder : BaseSetting.Builder
        {
            public bool? DefaultValue { get; set; }

            public Builder(string key, string name) : base(key, name) { }

            public override BaseSetting Build() => new BooleanSetting(this);
        }
    }
}
