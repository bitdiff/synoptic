namespace Synoptic.HelpUtilities
{
    public class ParameterHelp
    {
        public string FormattedLine { get; private set; }

        public ParameterHelp(string formattedLine)
        {
            FormattedLine = formattedLine;
        }
    }
}