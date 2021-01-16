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

        public sealed class Factory : IFactory
        {
            public BaseParameter Build(ParamJsonNode node) => new StringParameter(node);
        }
    }
}
