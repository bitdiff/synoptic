using System.Collections.Generic;
using System.Text;

namespace Synoptic
{
    public class CommandLineParseResult
    {
        public CommandLineParseResult(IEnumerable<CommandLineParameter> parsedParameters, string[] additionalParameters) : this(parsedParameters, additionalParameters, null) {}
        public CommandLineParseResult(IEnumerable<CommandLineParameter> parsedParameters, string[] additionalParameters, string message)
        {
            ParsedParameters = parsedParameters;
            AdditionalParameters = additionalParameters;
            Message = message;
        }

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