namespace Synoptic.HelpUtilities
{
    internal class ParameterHelp
    {
        public string FormattedLine { get; private set; }

        public ParameterHelp(string formattedLine)
        {
            FormattedLine = formattedLine;
        }
    }
}