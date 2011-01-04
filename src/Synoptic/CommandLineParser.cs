using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;

namespace Synoptic
{
    internal class CommandLineParser : ICommandLineParser
    {
        public CommandLineParseResult Parse(CommandManifest manifest, string[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new CommandException("Cannot derive command name from input.");
            }

            string commandName = args[0];
            args = args.Skip(1).ToArray();

            Command command = new CommandResolver().Resolve(manifest, commandName);

            if (command == null)
            {
                throw new CommandException(String.Format("There is no command with name '{0}'.", commandName));
            }

            var options = new OptionSet();
            var commandLineParameters = new List<CommandLineParameter>();

            foreach (ParameterInfoWrapper parameter in command.Parameters)
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

            return new CommandLineParseResult(command, commandLineParameters, extra.ToArray(), optionExceptionMessage);
        }
    }
}