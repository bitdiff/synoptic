using System.Collections.Generic;

namespace ConsoleWrapper.Synoptic
{
    public class CommandManifest
    {
        private readonly List<Command> _commands = new List<Command>();
        public List<Command> Commands
        {
            get { return _commands; }
        }
    }
}