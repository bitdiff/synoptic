using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal class CommandResolver : ICommandResolver
    {
        public CommandAction Resolve(CommandActionManifest actionManifest, string commandName)
        {
            return actionManifest.Commands.FirstOrDefault(c => c.Name.SimilarTo(commandName));
        }
    }

    internal class CommandResolver2
    {
        public CommandAction Resolve(List<CommandAction> actions, string actionName)
        {
            return actions.FirstOrDefault(c => c.Name.SimilarTo(actionName));
        }
    }
}