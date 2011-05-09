using System.Collections.Generic;

namespace Synoptic.HelpUtilities
{
    internal class CommandLineHelpGenerator
    {
        public static CommandLineHelp Generate(IEnumerable<CommandAction> commandActions)
        {
            return new CommandLineHelp(commandActions);
        }
    }
}