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
        public CommandAction Resolve(Command command, string action)
        {
            var actionManifest = new CommandActionActionFinder().FindInType(command.LinkedToType);
            return actionManifest.Commands.FirstOrDefault(c => c.Name.SimilarTo(action));
        }
    }
}