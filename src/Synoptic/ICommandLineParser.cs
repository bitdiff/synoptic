using System.Collections.Generic;

namespace Synoptic
{
    public interface ICommandLineParser
    {
        Dictionary<Command, CommandLineParseResult> Parse(CommandManifest manifest, string[] args);
    }
}