using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class IntegerParameter : BaseParameter
    {
        public int DefaultValue { get; }
        public IReadOnlyDictionary<int, string> Values { get; }

        private IntegerParameter(ParamJsonNode node) : base(node)
        {
            DefaultValue = node.Values.IntegerDefault();
            Values = ValueJsonNode.LoadValues(node.Values.List, n => n.IntegerValue());
        }

        private IntegerParameter(Builder builder) : base(builder)
        {
            DefaultValue = builder.DefaultValue;
            Values = builder.Values;
        }

        public sealed class Factory : IFactory
        {
            public BaseParameter Build(ParamJsonNode node) => new IntegerParameter(node);
        }

        public sealed class Builder : BaseBuilder
        {
            public int DefaultValue { get; set; }
            public Dictionary<int, string> Values { get; } = new Dictionary<int, string>();

            public Builder(string name) : base(name) { }

            public override BaseParameter Build() => new IntegerParameter(this);
        }
    }
}
