using System.Collections.Generic;
using Mono.Options;
using Synoptic.Exceptions;

namespace Synoptic
{
    internal class CommandLineParser : ICommandLineParser
    {
        public CommandLineParseResult Parse(CommandAction action, string[] args)
        {
            var options = new OptionSet();
            var commandLineParameters = new List<CommandLineParameter>();

            foreach (ParameterInfoWrapper parameter in action.Parameters)
            {
                ParameterInfoWrapper localParameter = parameter;
                options.Add(
                    localParameter.GetOptionPrototype(),
                    localParameter.Description,
                    parameterValue => commandLineParameters.Add(new CommandLineParameter(localParameter.Name, parameterValue))
                );
            }

            try
            {
                var extra = options.Parse(args);
                return new CommandLineParseResult(action, commandLineParameters, extra.ToArray());
            }
            catch (OptionException exception)
            {
                throw new CommandLineParseException(exception.Message, action);
            }
        }
    }
}