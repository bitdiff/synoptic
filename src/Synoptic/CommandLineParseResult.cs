using System.Collections.Generic;

namespace Synoptic
{
    internal class CommandLineParseResult
    {
        public CommandLineParseResult(CommandAction commandAction, 
            IEnumerable<CommandLineParameter> parsedParameters, 
            string[] additionalParameters) : this(commandAction, parsedParameters, additionalParameters, null) {}
        
        public CommandLineParseResult(CommandAction commandAction, 
            IEnumerable<CommandLineParameter> parsedParameters, 
            string[] additionalParameters, 
            string message)
        {
            CommandAction = commandAction;
            ParsedParameters = parsedParameters;
            AdditionalParameters = additionalParameters;
            Message = message;
        }

        public CommandAction CommandAction { get; private set; }
        public IEnumerable<CommandLineParameter> ParsedParameters { get; private set; }
        public string[] AdditionalParameters { get; private set; }
        public string Message { get; set; }
    }
}