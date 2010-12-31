using System.Collections.Generic;

namespace Synoptic
{
    internal interface ICommandLineParser
    {
        Dictionary<Command, CommandLineParseResult> Parse(CommandManifest manifest, string[] args);
    }
}