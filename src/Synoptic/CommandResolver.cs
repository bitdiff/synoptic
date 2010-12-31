using System.Linq;

namespace Synoptic
{
    public class CommandResolver : ICommandResolver
    {
        public Command Resolve(CommandManifest manifest, string commandName)
        {
            return manifest.Commands.FirstOrDefault(c => c.Name.EqualEnough(commandName));
        }
    }
}