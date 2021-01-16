using Defter.StarCitizen.ConfigDB.Json;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public class BaseCommand
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
    }
}
