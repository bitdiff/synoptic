using Synoptic.ConsoleFormat;

namespace Synoptic.Exceptions
{
    internal class CommandLineParseException : CommandParseExceptionBase
    {
        private readonly string _parserMessage;
        private readonly CommandAction _action;

        public CommandLineParseException(string parserMessage, CommandAction action)
        {
            _parserMessage = parserMessage;
            _action = action;
        }

        public string ParserMessage
        {
            get { return _parserMessage; }
        }

        public CommandAction Action
        {
            get { return _action; }
        }

        public override void Render()
        {
            ConsoleFormatter.Write(
                new ConsoleTable(new ConsoleCell(_parserMessage)));
        }
    }
}