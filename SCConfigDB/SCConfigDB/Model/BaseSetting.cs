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

        public interface IFactory
        {
            public BaseSetting Build(SettingJsonNode node);
        }
    }
}