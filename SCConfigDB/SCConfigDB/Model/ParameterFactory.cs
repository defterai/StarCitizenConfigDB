using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class ParameterFactory
    {
        private Dictionary<ValueJsonType, BaseParameter.IFactory> Factories { get; } = new Dictionary<ValueJsonType, BaseParameter.IFactory>();

        public ParameterFactory()
        {
            Register(ValueJsonType.Int, new IntegerParameter.Factory());
            Register(ValueJsonType.String, new StringParameter.Factory());
        }

        public void Register(ValueJsonType type, BaseParameter.IFactory factory) => Factories.Add(type, factory);

        public BaseParameter Build(ParamJsonNode node) => Factories[node.Values.Type].Build(node);
    }
}
