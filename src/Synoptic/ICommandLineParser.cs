using System;

namespace Synoptic
{
    internal interface ICommandLineParser
    {
        CommandLineParseResult Parse(CommandActionManifest actionManifest, string[] args, Func<string[], string[]> preProcessor);
    }
}