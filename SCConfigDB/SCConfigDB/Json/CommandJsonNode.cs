using System;
using Newtonsoft.Json;
using Defter.StarCitizen.ConfigDB.Collection;

namespace Defter.StarCitizen.ConfigDB.Json
{
    public sealed class CommandJsonNode : KeyedItemJsonNode
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; }
        [JsonProperty("category", Required = Required.Always)]
        public string Category { get; }
        [JsonProperty("desc")]
        public string? Description { get; }
        [JsonProperty("params")]
        public ParamJsonNode[]? Parameters { get; internal set; }

        [JsonConstructor]
        public CommandJsonNode(string key, string name, string category) : base(key)
        {
            Name = name;
            Category = category;
        }

        public CommandJsonNode(CommandJsonNode node, CommandTranslateJsonNode translateNode)
            : base(node.Key)
        {
            if (!node.IsKeyEqual(translateNode.Key))
            {
                throw new InvalidOperationException($"translate node key {translateNode.Key} is not equal to {node.Key}");
            }
            if ((node.Parameters == null || node.Parameters.Length == 0) && translateNode.Parameters != null)
            {
                throw new InvalidOperationException($"translate node {translateNode.Key} should not contains params");
            }
            if (node.Parameters != null && translateNode.Parameters != null && node.Parameters.Length != translateNode.Parameters.Length)
            {
                throw new InvalidOperationException($"translate node {translateNode.Key} should contains same params count");
            }
            Name = translateNode.Name;
            Category = node.Category;
            Description = translateNode.Description ?? node.Description;
            if (node.Parameters != null)
            {
                Parameters = ArrayHelper.Clone(node.Parameters);
                if (translateNode.Parameters != null)
                {
                    for (int i = 0; i < translateNode.Parameters.Length; i++)
                    {
                        Parameters[i] = node.Parameters[i].TranslateWith(translateNode.Parameters[i]);
                    }
                }
            }
        }

        public CommandJsonNode TranslateWith(CommandTranslateJsonNode translateNode) =>
            new CommandJsonNode(this, translateNode);
    }
}
