using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Collection;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class SettingCategory
    {
        public string Name { get; }
        public IReadOnlyDictionary<string, BaseSetting> Settings { get; }

        private SettingCategory(Builder builder)
        {
            Name = builder.Name;
            Settings = builder.Settings;
        }

        public class Builder
        {
            public string Name { get; }

            public IOrderedDictionary<string, BaseSetting> Settings { get; } = new OrderedDictionary<string, BaseSetting>();

            public Builder(string name)
            {
                Name = name;
            }

            public void AddSetting(BaseSetting setting) => Settings.Add(setting.Key, setting);

            public SettingCategory Build() => new SettingCategory(this);
        }
    }
}
