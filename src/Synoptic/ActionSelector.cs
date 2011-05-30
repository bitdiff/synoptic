using System.Linq;

namespace Synoptic
{
    internal class ActionSelector
    {
        private readonly ICommandActionFinder _commandActionFinder = new CommandActionFinder();

        public CommandAction Select(string actionName, Command command)
        {
            var availableActions = _commandActionFinder.FindInCommand(command);
            CommandAction action = new MatchSelector<CommandAction>().Match(actionName, availableActions, c => c.Name) ??
                                   availableActions.FirstOrDefault(a => a.IsDefault);

            if (action != null)
                return action;

            var exception = new ActionNotFoundException(actionName, command);
            exception.AvailableActions.AddRange(availableActions);

            var possibleActions = new MatchSelector<CommandAction>().PartialMatch(actionName, availableActions, c => c.Name);
            exception.PossibleActions.AddRange(possibleActions);

            throw exception;
        }
    }
}