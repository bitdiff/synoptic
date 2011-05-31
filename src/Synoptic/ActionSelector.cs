using System.Collections.Generic;
using System.Linq;
using Synoptic.Exceptions;

namespace Synoptic
{
    internal class ActionSelector
    {
        public CommandAction Select(string actionName, Command command, IEnumerable<CommandAction> availableActions)
        {
            CommandAction action = new MatchSelector<CommandAction>().Match(actionName, availableActions, c => c.Name) ??
                                   availableActions.FirstOrDefault(a => a.IsDefault);

            if (action != null)
                return action;

            var exception = new CommandActionNotFoundException(actionName, command);
            exception.AvailableActions.AddRange(availableActions);

            var possibleActions = new MatchSelector<CommandAction>().PartialMatch(actionName, availableActions, c => c.Name);
            exception.PossibleActions.AddRange(possibleActions);

            throw exception;
        }
    }
}