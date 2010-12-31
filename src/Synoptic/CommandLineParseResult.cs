using System.Collections.Generic;
using System.Text;

namespace Synoptic
{
    internal class CommandLineParseResult
    {
        public CommandLineParseResult(Command command, IEnumerable<CommandLineParameter> parsedParameters, string[] additionalParameters) : this(command, parsedParameters, additionalParameters, null) {}
        public CommandLineParseResult(Command command, IEnumerable<CommandLineParameter> parsedParameters, string[] additionalParameters, string message)
        {
            Command = command;
            ParsedParameters = parsedParameters;
            AdditionalParameters = additionalParameters;
            Message = message;
        }

        public Command Command { get; private set; }
        public IEnumerable<CommandLineParameter> ParsedParameters { get; private set; }
        public string[] AdditionalParameters { get; private set; }
        public string Message { get; set; }
        public bool WasSuccessfullyParsed { get { return string.IsNullOrEmpty(Message); } }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendFormat("[ParsedParameters]\n");
            foreach (var commandLineParameter in ParsedParameters)
            {
                str.AppendFormat("  [CommandLineParameter] {0}:{1}", commandLineParameter.Name,
                                 commandLineParameter.Value);
            }

            str.AppendFormat("[AdditionalParameters]\n");
            foreach (var additionalParameter in AdditionalParameters)
            {
                str.AppendFormat("  {0}", additionalParameter);
            }

            return str.ToString();
        }
    }
}