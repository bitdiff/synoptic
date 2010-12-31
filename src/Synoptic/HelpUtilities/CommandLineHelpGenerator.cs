namespace ConsoleWrapper.Synoptic.HelpUtilities
{
    public class CommandLineHelpGenerator
    {
        public static CommandLineHelp Generate(CommandManifest manifest)
        {
            return new CommandLineHelp(manifest.Commands);
        }
    }
}