namespace Synoptic.HelpUtilities
{
    internal class CommandLineHelpGenerator
    {
        public static CommandLineHelp Generate(CommandActionManifest actionManifest)
        {
            return new CommandLineHelp(actionManifest.Commands);
        }
    }
}