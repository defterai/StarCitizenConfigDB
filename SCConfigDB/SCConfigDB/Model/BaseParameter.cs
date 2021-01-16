using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public class BaseParameter
    {
        public string Name { get; }
        public string? Description { get; }

        protected BaseParameter(ParamJsonNode node)
        {
            Name = node.Name;
            Description = node.Description;
        }

        public interface IFactory
        {
            public BaseParameter Build(ParamJsonNode node);
        }
    }
}
