using System.Collections.Generic;

namespace ConsoleWrapper.Synoptic
{
    public interface ICommandLineParser
    {
        Dictionary<Command, CommandLineParseResult> Parse(CommandManifest manifest, string[] args);
    }
}