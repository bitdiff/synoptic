using System.Collections.Generic;

namespace Synoptic
{
    internal interface ICommandLineParser
    {
        CommandLineParseResult Parse(CommandManifest manifest, string[] args);
    }
}