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

        protected BaseParameter(BaseBuilder builder)
        {
            Name = builder.Name;
            Description = builder.Description;
        }

        public interface IFactory
        {
            public BaseParameter Build(ParamJsonNode node);
        }

        public abstract class BaseBuilder
        {
            public string Name { get; }
            public string? Description { get; set; }

            public BaseBuilder(string name)
            {
                Name = name;
            }

            public abstract BaseParameter Build();
        }
    }
}
