using System.Collections.Generic;
using System.Linq;

namespace Synoptic
{
    internal class CommandResolver : ICommandResolver
    {
        public CommandAction Resolve(IEnumerable<CommandAction> availableActions, string actionName)
        {
            return availableActions.FirstOrDefault(c => c.Name.SimilarTo(actionName));
        }
    }
}