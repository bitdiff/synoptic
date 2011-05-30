namespace Synoptic
{
    internal interface ICommandLineParser
    {
        CommandLineParseResult Parse(CommandAction action,  string[] args);
    }
}