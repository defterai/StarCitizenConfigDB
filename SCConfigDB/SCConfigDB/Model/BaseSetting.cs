using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public abstract class BaseSetting
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

        protected BaseSetting(Builder builder)
        {
            Key = builder.Key;
            Name = builder.Name;
            Description = builder.Description;
        }

        public abstract void ExctractValueNodes(List<ValueJsonNode> nodes);

        public abstract SettingValuesJsonNode GetValuesNode();

        public interface IFactory
        {
            BaseSetting Build(SettingJsonNode node);
        }

        public abstract class Builder
        {
            public string Key { get; }
            public string Name { get; }
            public string? Description { get; }

            public Builder(string key, string name)
            {
                Key = key;
                Name = name;
            }

            public abstract BaseSetting Build();
        }
    }
}
