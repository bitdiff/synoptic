using System.Collections.Generic;

namespace Synoptic
{
    internal interface ICommandActionFinder
    {
        IEnumerable<CommandAction> FindInCommand(Command command);
    }
}