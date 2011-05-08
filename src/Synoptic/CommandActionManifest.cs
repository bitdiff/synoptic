using System.Collections.Generic;

namespace Synoptic
{
    internal class CommandActionManifest
    {
        private readonly List<CommandAction> _commands = new List<CommandAction>();
        public List<CommandAction> Commands
        {
            get { return _commands; }
        }
    }
}