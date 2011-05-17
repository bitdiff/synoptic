using System;
using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal class CommandSelector
    {
        public Command Select(string commandName, List<Command> availableCommands)
        {
            var command = new MatchSelector<Command>().Match(commandName, availableCommands, c => c.Name);
            if (command != null)
                return command;

            var message = String.Format("'{0}' is not a valid command.", commandName);

            var possibleCommands = new MatchSelector<Command>().PartialMatch(commandName, availableCommands, c => c.Name);

            if (possibleCommands.Count() > 0)
            {
                var possibleCommandsFormatted = string.Join(" or ",
                    possibleCommands.Select(c => String.Format("'{0}'", c.Name.ToHyphened())).ToArray());

                throw new CommandActionException(String.Format("{0}\n\nDid you mean:\n\t{1}?", message, possibleCommandsFormatted));
            }

            throw new CommandActionException(message);
        }
    }
}