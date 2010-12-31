using System.Collections.Generic;
using Mono.Options;

namespace Synoptic
{
    public class CommandLineParser : ICommandLineParser
    {
        public Dictionary<Command, CommandLineParseResult> Parse(CommandManifest manifest, string[] args)
        {
            var result = new Dictionary<Command, CommandLineParseResult>();

            foreach (var command in manifest.Commands)
            {
                var options = new OptionSet();
                var commandLineParameters = new List<CommandLineParameter>();

                foreach (var parameter in command.Parameters)
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
                    result.Add(command, new CommandLineParseResult(commandLineParameters, extra.ToArray(), null));
                }
                catch (OptionException exception) { result.Add(command, new CommandLineParseResult(commandLineParameters, null, exception.Message)); }
            }

            return result;
        }
    }
}