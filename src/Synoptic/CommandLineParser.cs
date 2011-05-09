using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;

namespace Synoptic
{
    internal class CommandLineParser : ICommandLineParser
    {
        private readonly ICommandActionFinder _commandActionFinder = new CommandActionFinder();
        private readonly ICommandResolver _commandResolver = new CommandResolver();

        public CommandLineParseResult Parse(Command command, string[] args)
        {
            var availableActions = _commandActionFinder.FindInCommand(command);
            
            CommandAction selectedAction;

            if(args != null && args.Length > 0)
            {
                selectedAction = _commandResolver.Resolve(availableActions, args[0]);
                args = args.Skip(1).ToArray();
            }
            else
            {
                selectedAction = availableActions.FirstOrDefault(a => a.IsDefault);
            }
                
            if (selectedAction == null)
            {
                throw new CommandActionException(String.Format("One of the following actions is required for command '{0}': {1}.", 
                    command.Name, String.Join(", ", availableActions.Select(a => a.Name).ToArray())));
            }

            var options = new OptionSet();
            var commandLineParameters = new List<CommandLineParameter>();

            foreach (ParameterInfoWrapper parameter in selectedAction.Parameters)
            {
                ParameterInfoWrapper localParameter = parameter;
                options.Add(
                    localParameter.GetOptionPrototype(),
                    localParameter.Description,
                    parameterValue => commandLineParameters.Add(new CommandLineParameter(localParameter.Name, parameterValue))
                );
            }

            var extra = new List<string>();
            string optionExceptionMessage = null;

            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException exception)
            {
                optionExceptionMessage = exception.Message;
            }

            return new CommandLineParseResult(selectedAction, commandLineParameters, extra.ToArray(), optionExceptionMessage);
        }
    }
}