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
            var possibleCommands = string.Join(" or ", 
                availableCommands.Select(c => String.Format("'{0}'", c.Name.ToHyphened())).ToArray());

            if (possibleCommands.Count() > 0)
            {
                throw new CommandActionException(String.Format("{0}\n\nDid you mean:\n\t{1}?", message, possibleCommands));
            }

            throw new CommandActionException(message);
        }
    }
}