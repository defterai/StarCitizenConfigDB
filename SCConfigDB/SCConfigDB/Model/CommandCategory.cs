using System.Collections.Generic;
using Defter.StarCitizen.ConfigDB.Collection;

namespace Defter.StarCitizen.ConfigDB.Model
{
    public sealed class CommandCategory
    {
        public string Name { get; }
        public IReadOnlyDictionary<string, BaseCommand> Commands { get; }

        private CommandCategory(Builder builder)
        {
            Name = builder.Name;
            Commands = builder.Commands;
        }

        public sealed class Builder
        {
            public string Name { get; }

            public IOrderedDictionary<string, BaseCommand> Commands { get; } = new OrderedDictionary<string, BaseCommand>();

            public Builder(string name)
            {
                Name = name;
            }

            public void AddCommand(BaseCommand command) => Commands.Add(command.Key, command);

            public CommandCategory Build() => new CommandCategory(this);
        }
    }
}
