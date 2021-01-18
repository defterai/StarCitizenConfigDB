using Newtonsoft.Json;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class CommandsTranslateJsonNode : CategoryItemsJsonNode<CommandTranslateJsonNode> {

        [JsonConstructor]
        public CommandsTranslateJsonNode() { }

        private CommandsTranslateJsonNode(Builder builder) : base(builder) { }

        public new sealed class Builder : CategoryItemsJsonNode<CommandTranslateJsonNode>.Builder
        {
            public new CommandsTranslateJsonNode Build() => new CommandsTranslateJsonNode(this);
        }
    }
}
