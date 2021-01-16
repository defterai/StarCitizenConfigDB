using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class BooleanSetting : BaseSetting
    {
        public bool DefaultValue { get; }

        private BooleanSetting(SettingJsonNode node) : base(node)
        {
            DefaultValue = node.Values.BooleanDefault();
        }

        public sealed class Factory : IFactory
        {
            public BaseSetting Build(SettingJsonNode node) => new BooleanSetting(node);
        }
    }
}
