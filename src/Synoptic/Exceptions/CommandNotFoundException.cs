using System;
using System.Collections.Generic;

namespace Synoptic.Exceptions
{
    internal class CommandNotFoundException : ApplicationException
    {
        private readonly string _commandName;
        private readonly List<Command> _possibleCommands = new List<Command>();
        private readonly List<Command> _availableCommands = new List<Command>();

        public CommandNotFoundException(string commandName, IEnumerable<Command> possibleCommands)
            : this(commandName)
        {
            _possibleCommands.AddRange(possibleCommands);
            _commandName = commandName;
        }

        public CommandNotFoundException(string commandName)
        {
            _commandName = commandName;
        }

        public string CommandName
        {
            get { return _commandName; }
        }

        public List<Command> PossibleCommands
        {
            get { return _possibleCommands; }
        }
        
        public List<Command> AvailableCommands
        {
            get { return _availableCommands; }
        }
    }
}