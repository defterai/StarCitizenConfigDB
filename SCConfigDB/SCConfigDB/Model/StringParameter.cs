using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class StringParameter : BaseParameter
    {
        public string DefaultValue { get; }
        public IReadOnlyDictionary<string, string> Values { get; }

        private StringParameter(ParamJsonNode node) : base(node)
        {
            DefaultValue = node.Values.DefaultValue;
            Values = ValueJsonNode.LoadValues(node.Values.List, n => n.Value);
        }

        private StringParameter(Builder builder) : base(builder)
        {
            DefaultValue = builder.DefaultValue;
            Values = builder.Values;
        }

        public sealed class Factory : IFactory
        {
            public BaseParameter Build(ParamJsonNode node) => new StringParameter(node);
        }

        public sealed class Builder : BaseBuilder
        {
            public string DefaultValue { get; set; } = string.Empty;
            public Dictionary<string, string> Values { get; } = new Dictionary<string, string>();

            public Builder(string name) : base(name) { }

            public override BaseParameter Build() => new StringParameter(this);
        }
    }
}
