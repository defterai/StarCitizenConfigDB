using Newtonsoft.Json;
using Defter.StarCitizen.ConfigDB.Collection;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class SettingsJsonNode : CategoryItemsJsonNode<SettingJsonNode>
    {
        [JsonConstructor]
        public SettingsJsonNode() { }

        private SettingsJsonNode(Builder builder) : base(builder) { }

        private SettingsJsonNode(SettingsJsonNode node, SettingsTranslateJsonNode translateNode)
            : base(ArrayHelper.Clone(node.Categories), ArrayHelper.Clone(node.Items))
        {
            foreach (var categoryTranslateNode in translateNode.Categories)
            {
                int index = node.GetCategoryIndex(categoryTranslateNode.Key);
                if (index != -1)
                {
                    Categories[index] = node.Categories[index].TranslateWith(categoryTranslateNode);
                }
            }
            foreach (var itemTranslateNode in translateNode.Items)
            {
                int index = node.GetItemIndex(itemTranslateNode.Key);
                if (index != -1)
                {
                    Items[index] = node.Items[index].TranslateWith(itemTranslateNode);
                }
            }
        }

        public SettingsJsonNode TranslateWith(SettingsTranslateJsonNode translateNode) => new SettingsJsonNode(this, translateNode);

        public new sealed class Builder : CategoryItemsJsonNode<SettingJsonNode>.Builder
        {
            public new SettingsJsonNode Build() => new SettingsJsonNode(this);
        }
    }
}
