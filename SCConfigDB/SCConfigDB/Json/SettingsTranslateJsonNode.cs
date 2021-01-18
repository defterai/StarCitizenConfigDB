using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class SettingsTranslateJsonNode : CategoryItemsJsonNode<SettingTranslateJsonNode>
    {

        [JsonConstructor]
        public SettingsTranslateJsonNode() { }

        private SettingsTranslateJsonNode(Builder builder) : base(builder) { }

        public new sealed class Builder : CategoryItemsJsonNode<SettingTranslateJsonNode>.Builder
        {
            public new SettingsTranslateJsonNode Build() => new SettingsTranslateJsonNode(this);
        }
    }
}
