using System.Collections.Generic;

namespace Synoptic
{
    internal class CommandManifest
    {
        private readonly List<Command> _commands = new List<Command>();
        public List<Command> Commands
        {
            get { return _commands; }
        }
    }
}