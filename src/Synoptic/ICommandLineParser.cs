namespace Synoptic
{
    internal interface ICommandLineParser
    {
        CommandLineParseResult Parse(Command command, string[] args);
    }
}