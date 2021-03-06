using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class ConfigData
    {
        public IReadOnlyDictionary<string, CommandCategory> CommandCategories { get; }
        public IReadOnlyDictionary<string, SettingCategory> SettingCategories { get; }

        private ConfigData(Builder builder)
        {
            CommandCategories = builder.CommandCategoryBuilders.ToDictionary(entry => entry.Key, entry => entry.Value.Build());
            SettingCategories = builder.SettingCategoryBuilders.ToDictionary(entry => entry.Key, entry => entry.Value.Build());
            VerifySettings();
        }

        public BaseCommand? GetCommand(string commandKey)
        {
            foreach (var category in CommandCategories.Values)
            {
                if (category.Commands.TryGetValue(commandKey, out var comand))
                {
                    return comand;
                }
            }
            return null;
        }

        public BaseSetting? GetSetting(string settingKey)
        {
            foreach (var category in SettingCategories.Values)
            {
                if (category.Settings.TryGetValue(settingKey, out var setting))
                {
                    return setting;
                }
            }
            return null;
        }

        private void VerifySettings()
        {
            HashSet<string> settingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var category in SettingCategories.Values)
            {
                foreach (var settingName in category.Settings.Keys)
                {
                    if (!settingNames.Add(settingName))
                    {
                        throw new InvalidDataException($"Duplicate setting key is not allowed: {settingName}");
                    }
                }
            }
        } 

        public sealed class Builder
        {
            public Dictionary<string, CommandCategory.Builder> CommandCategoryBuilders { get; }
            public Dictionary<string, SettingCategory.Builder> SettingCategoryBuilders { get; }

            public Builder(Dictionary<string, CommandCategory.Builder> commandCategoryBuilders,
                Dictionary<string, SettingCategory.Builder> settingCategoryBuilders) {
                CommandCategoryBuilders = commandCategoryBuilders;
                SettingCategoryBuilders = settingCategoryBuilders;
            }

            public Builder(ConfigDataJsonNode node, SettingFactory settingFactory, ParameterFactory parameterFactory)
            {
                CommandCategoryBuilders = CreateCategoryBuilders(node.Commands.Categories, n => new CommandCategory.Builder(n.Name));
                SettingCategoryBuilders = CreateCategoryBuilders(node.Settings.Categories, n => new SettingCategory.Builder(n.Name));
                foreach (var commandNode in node.Commands.Items)
                {
                    var builder = GetCategoryBuilder(CommandCategoryBuilders, commandNode.Category);
                    builder.AddCommand(new BaseCommand(commandNode, parameterFactory));
                }
                foreach (var settingNode in node.Settings.Items)
                {
                    var builder = GetCategoryBuilder(SettingCategoryBuilders, settingNode.Category);
                    builder.AddSetting(settingFactory.Build(settingNode));
                }
            }

            public ConfigData Build() => new ConfigData(this);

            private static Dictionary<string, T> CreateCategoryBuilders<T>(CategoryJsonNode[] categories,
                Func<CategoryJsonNode, T> builder)
            {
                var builders = new Dictionary<string, T>(categories.Length);
                foreach (var categoryNode in categories)
                {
                    builders.Add(categoryNode.Key, builder(categoryNode));
                }
                return builders;
            }

            private static T GetCategoryBuilder<T>(IReadOnlyDictionary<string, T> builders, string category)
            {
                if (builders.TryGetValue(category, out var builder))
                {
                    return builder;
                }
                throw new InvalidDataException($"Category '{category}' was not present in categories");
            }
        }
    }
}
