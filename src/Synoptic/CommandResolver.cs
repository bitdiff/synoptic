using System.Linq;

namespace Synoptic
{
    internal class CommandResolver : ICommandResolver
    {
        public Command Resolve(CommandManifest manifest, string commandName)
        {
            return manifest.Commands.FirstOrDefault(c => c.Name.SimilarTo(commandName));
        }
    }
}