using Synoptic.ConsoleFormat;

namespace Synoptic.Exceptions
{
    public class CommandParameterInvalidException : CommandParseExceptionBase
    {
        private readonly string _message;

        public CommandParameterInvalidException(string message)
        {
            _message = message;
        }

        public override void Render()
        {
            ConsoleFormatter.Write(new ConsoleTable(
                                       new ConsoleCell("Unable to run action with supplied parameters.").WithPadding(0))
                                       .AddEmptyRow()
                                       .AddRow(new ConsoleCell(_message).WithPadding(0)));
        }
    }
}