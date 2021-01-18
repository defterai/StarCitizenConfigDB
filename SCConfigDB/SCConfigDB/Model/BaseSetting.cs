using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public class BaseSetting
    {
        public string Key { get; }
        public string Name { get; }
        public string? Description { get; }

        protected BaseSetting(SettingJsonNode node)
        {
            Key = node.Key;
            Name = node.Name;
            Description = node.Description;
        }

        protected BaseSetting(BaseBuilder builder)
        {
            Key = builder.Key;
            Name = builder.Name;
            Description = builder.Description;
        }

        public interface IFactory
        {
            public BaseSetting Build(SettingJsonNode node);
        }

        public abstract class BaseBuilder
        {
            public string Key { get; }
            public string Name { get; }
            public string? Description { get; }

            public BaseBuilder(string key, string name)
            {
                Key = key;
                Name = name;
            }

            public abstract BaseSetting Build();
        }
    }
}
