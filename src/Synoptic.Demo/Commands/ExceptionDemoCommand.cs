using System;

namespace Synoptic.Demo.Commands
{
    [Command(Name = "exception", Description = "Demonstrates the different ways in which exceptions can be handled.")]
    public class ExceptionDemoCommand
    {
        [CommandAction(Name = "ex", Description = "Generates an exception.")]
        public void GenerateException()
        {
            throw new Exception("Exception generated.");
        }

        [CommandAction(Name = "cex", Description = "Generates a CommandException.")]
        public void GenerateCommandException()
        {
            throw new CommandActionException("This is caught internally.");
        }
    }
}