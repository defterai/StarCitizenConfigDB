using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public abstract class BaseParameter
    {
        public string Name { get; }
        public string? Description { get; }

        protected BaseParameter(ParamJsonNode node)
        {
            Name = node.Name;
            Description = node.Description;
        }

        protected BaseParameter(Builder builder)
        {
            Name = builder.Name;
            Description = builder.Description;
        }

        public abstract void ExctractValueNodes(List<ValueJsonNode> nodes);

        public abstract ValuesJsonNode GetValuesNode();

        public interface IFactory
        {
            BaseParameter Build(ParamJsonNode node);
        }

        public abstract class Builder
        {
            public string Name { get; }
            public string? Description { get; set; }

            public Builder(string name)
            {
                Name = name;
            }

            public abstract BaseParameter Build();
        }
    }
}
