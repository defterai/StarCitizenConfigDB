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

        public sealed class Factory : IFactory
        {
            public BaseParameter Build(ParamJsonNode node) => new IntegerParameter(node);
        }
    }
}
