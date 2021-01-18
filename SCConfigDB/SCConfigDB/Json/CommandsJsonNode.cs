using Newtonsoft.Json;
using Defter.StarCitizen.ConfigDB.Collection;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class CommandsJsonNode : CategoryItemsJsonNode<CommandJsonNode>
    {
        [JsonConstructor]
        public CommandsJsonNode() { }

        private CommandsJsonNode(Builder builder) : base(builder) { }

        private CommandsJsonNode(CommandsJsonNode node, CommandsTranslateJsonNode translateNode)
            : base(ArrayHelper.Clone(node.Categories), ArrayHelper.Clone(node.Items))
        {
            foreach (var categoryTranslateNode in translateNode.Categories)
            {
                int? index = node.GetCategoryIndex(categoryTranslateNode.Key);
                if (index.HasValue)
                {
                    Categories[index.Value] = node.Categories[index.Value].TranslateWith(categoryTranslateNode);
                }
            }
            foreach (var itemTranslateNode in translateNode.Items)
            {
                int? index = node.GetItemIndex(itemTranslateNode.Key);
                if (index.HasValue)
                {
                    Items[index.Value] = node.Items[index.Value].TranslateWith(itemTranslateNode);
                }
            }
        }

        public CommandsJsonNode TranslateWith(CommandsTranslateJsonNode translateNode) => new CommandsJsonNode(this, translateNode);

        public new sealed class Builder : CategoryItemsJsonNode<CommandJsonNode>.Builder
        {
            public new CommandsJsonNode Build() => new CommandsJsonNode(this);
        }
    }
}
