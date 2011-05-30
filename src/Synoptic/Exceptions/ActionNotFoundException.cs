using System;
using System.Collections.Generic;

namespace Synoptic
{
    internal class ActionNotFoundException : ApplicationException
    {
        private readonly string _actionName;
        private readonly Command _command;
        private readonly List<CommandAction> _availableActions = new List<CommandAction>();
        private readonly List<CommandAction> _possibleActions = new List<CommandAction>();

        public ActionNotFoundException(string actionName, Command command)
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
    }
}