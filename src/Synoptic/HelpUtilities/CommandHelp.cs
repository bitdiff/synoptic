using System.Collections.Generic;

namespace Synoptic.HelpUtilities
{
    public class CommandHelp
    {
        private readonly List<ParameterHelp> _parameters = new List<ParameterHelp>();

        public CommandHelp(string formattedLine)
        {
            FormattedLine = formattedLine;
        }

        public List<ParameterHelp> Parameters
        {
            get { return _parameters; }
        }

        public string FormattedLine { get; private set; }
    }
}