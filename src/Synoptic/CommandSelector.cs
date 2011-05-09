using System;
using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal class CommandSelector
    {
        public Command Select(string commandName, List<Command> availableCommands)
        {
            var commands = new PartialMatchSelector<Command>().Match(commandName, availableCommands, c => c.Name);

            if (commands.Count() == 0)
                throw new CommandActionException(String.Format("Command '{0}' not found.", commandName));

            if (commands.Count() == 1)
                return commands.First();

            var ambiguous = string.Join(", ", commands.Select(c => c.Name).ToArray());
            throw new CommandActionException(String.Format("Command '{0}' is ambiguous as there are multiple possibilities: {1}.", commandName, ambiguous));
        }
    }
}