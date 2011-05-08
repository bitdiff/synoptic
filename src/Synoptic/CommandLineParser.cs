using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;

namespace Synoptic
{
    internal class CommandLineParser2
    {
        public CommandLineParseResult Parse(Command command, string[] args)
        {
            var actions = new CommandActionActionFinder().FindInType(command.LinkedToType).Commands;
            
            if (args == null || args.Length == 0)
            {
                throw new CommandActionException(String.Format("Did you mean {0}?",
                                                 String.Join(" or ", actions.Select(a => a.Name).ToArray())));
            }

            string commandName = args[0];
            args = args.Skip(1).ToArray();

            CommandAction commandAction = new CommandResolver2().Resolve(actions, commandName);

            if (commandAction == null)
            {
                throw new CommandActionException(String.Format("There is no action with name '{0}'. Did you mean {1}?", commandName, String.Join(" or ", actions.Select(a => a.Name).ToArray())));
            }

            var options = new OptionSet();
            var commandLineParameters = new List<CommandLineParameter>();

            foreach (ParameterInfoWrapper parameter in commandAction.Parameters)
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

            return new CommandLineParseResult(commandAction, commandLineParameters, extra.ToArray(), optionExceptionMessage);
        }
    }

    internal class CommandLineParser : ICommandLineParser
    {
        public CommandLineParseResult Parse(CommandActionManifest actionManifest, string[] args, Func<string[], string[]> preProcessor)
        {
            if (args == null || args.Length == 0)
            {
                throw new CommandActionException("Cannot derive command name from input.");
            }

            string commandName = args[0];
            args = args.Skip(1).ToArray();

            CommandAction commandAction = new CommandResolver().Resolve(actionManifest, commandName);

            if (commandAction == null)
            {
                throw new CommandActionException(String.Format("There is no command with name '{0}'.", commandName));
            }

            var options = new OptionSet();
            var commandLineParameters = new List<CommandLineParameter>();

            foreach (ParameterInfoWrapper parameter in commandAction.Parameters)
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

            return new CommandLineParseResult(commandAction, commandLineParameters, extra.ToArray(), optionExceptionMessage);
        }
    }
}