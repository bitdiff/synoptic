using System.Collections.Generic;
using Mono.Options;

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

            return new CommandLineParseResult(action, commandLineParameters, extra.ToArray(), optionExceptionMessage);
        }
    }
}