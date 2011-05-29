using System;
using System.Collections.Generic;
using System.Linq;
using Synoptic.ConsoleFormat;

namespace Synoptic.Exceptions
{
    internal class CommandActionNotFoundException : CommandParseExceptionBase
    {
        private readonly string _actionName;
        private readonly Command _command;
        private readonly List<CommandAction> _availableActions = new List<CommandAction>();
        private readonly List<CommandAction> _possibleActions = new List<CommandAction>();

        public CommandActionNotFoundException(string actionName, Command command)
        {
            _actionName = actionName;
            _command = command;
        }

        public string ActionName
        {
            get { return _actionName; }
        }

        public Command Command
        {
            get { return _command; }
        }

        public List<CommandAction> AvailableActions
        {
            get { return _availableActions; }
        }

        public List<CommandAction> PossibleActions
        {
            get { return _possibleActions; }
        }

        public override void Render()
        {
            if (ActionName == null)
            {
                ConsoleFormatter.Write(new ConsoleTable()
                                           .AddRow(
                                               new ConsoleCell("You must specify an action name for command '{0}'.",
                                                               Command.Name).WithPadding(0)));
            }
            else
            {
                ConsoleFormatter.Write(new ConsoleTable()
                                           .AddRow(
                                               new ConsoleCell("'{0}' is not a valid action for command '{1}'.",
                                                               ActionName, Command.Name).WithPadding(0)));
            }

            var formattedActionList = string.Join(" or ",
                                                  (PossibleActions.Count > 0
                                                       ? PossibleActions
                                                       : AvailableActions).Select(
                                                           c => String.Format("'{0}'", c.Name)).ToArray());

            ConsoleFormatter.Write(new ConsoleTable()
                                       .AddEmptyRow()
                                       .AddRow(
                                           new ConsoleCell("Did you mean:").WithPadding(0))
                                       .AddRow(
                                           new ConsoleCell(formattedActionList)));

        }
    }
}