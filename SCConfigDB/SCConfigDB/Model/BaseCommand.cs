using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class BaseCommand
    {
        public string Key { get; }
        public string Name { get; }
        public string? Description { get; }
        public BaseParameter[]? Parameters { get; }

        public BaseCommand(CommandJsonNode node, ParameterFactory parameterFactory)
        {
            Key = node.Key;
            Name = node.Name;
            Description = node.Description;
            if (node.Parameters != null && node.Parameters.Length != 0)
            {
                Parameters = new BaseParameter[node.Parameters.Length];
                for (int i = 0; i < node.Parameters.Length; i++)
                {
                    Parameters[i] = parameterFactory.Build(node.Parameters[i]);
                }
            }
        }

        private BaseCommand(Builder builder)
        {
            Key = builder.Key;
            Name = builder.Name;
            Description = builder.Description;
            if (builder.Parameters.Count != 0)
            {
                Parameters = builder.Parameters.ToArray();
            }
        }

        public sealed class Builder
        {
            public string Key { get; }
            public string Name { get; }
            public string? Description { get; set; }
            public List<BaseParameter> Parameters { get; } = new List<BaseParameter>();

            public Builder(string key, string name)
            {
                Key = key;
                Name = name;
            }

            public BaseCommand Build() => new BaseCommand(this);
        }
    }
}
