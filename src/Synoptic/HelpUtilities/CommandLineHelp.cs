using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleWrapper.Synoptic.HelpUtilities
{
    public class CommandLineHelp
    {
        private readonly List<CommandHelp> _commands = new List<CommandHelp>();
        private const int SpacingWidth = 3;

        public CommandLineHelp(IEnumerable<Command> commands)
        {
            var spacer = new string(' ', SpacingWidth);
            MaximumCommandNameLength = commands.Max(c => c.Name.Length);
            
            // Determine the length of the longest prototype across all commands.
            foreach(var command in commands)
            {
                int length = 0;
                if (command.Parameters != null && command.Parameters.Count > 0)
                    length = command.Parameters.Max(p => p.GetOptionPrototypeHelp().Length);
                if (length > MaximumPrototypeLength)
                    MaximumPrototypeLength = length;
            }

            foreach(var command in commands)
            {
                var commandHelp = new CommandHelp(String.Format("{1}{0," + -MaximumCommandNameLength  + "}{1}{2}", command.Name.ToHyphened(), spacer, command.Description));
                foreach(var parameter in command.Parameters)
                {
                    commandHelp.Parameters.Add(new ParameterHelp(String.Format("{1}{1}{0," + -MaximumPrototypeLength + "}{1}{2}", parameter.GetOptionPrototypeHelp(), spacer, parameter.Description)));    
                }
                Commands.Add(commandHelp);
            }
        }
        
        public int MaximumCommandNameLength { get; private set; }
        public int MaximumPrototypeLength { get; private set; }

        public List<CommandHelp> Commands
        {
            get { return _commands; }
        }
    }
}