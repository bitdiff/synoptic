using System.Collections.Generic;

namespace Synoptic
{
    internal interface ICommandResolver
    {
        CommandAction Resolve(IEnumerable<CommandAction> actionManifest, string commandName);
    }
}