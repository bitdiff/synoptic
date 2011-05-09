using System.Collections.Generic;

namespace Synoptic
{
    internal interface ICommandResolver
    {
        CommandAction Resolve(IEnumerable<CommandAction> availableActions, string commandName);
    }
}