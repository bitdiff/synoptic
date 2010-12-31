using System;
using System.Diagnostics;
using System.Reflection;
using Synoptic.HelpUtilities;

namespace Synoptic
{
    public class CommandRunner
    {
        private CommandManifest _manifest;
        private readonly ICommandFinder _finder;
        private CommandLineHelp _help;

        public CommandRunner()
        {
            _finder = new CommandFinder();
        }

        public CommandRunner WithCommand<T>()
        {
            CommandManifest findInType = _finder.FindInType(typeof (T));

            if (_manifest == null)
                _manifest = findInType;
            else
                _manifest.Commands.AddRange(findInType.Commands);

            return this;
        }

        public CommandRunner WithCommandsFromAssembly(Assembly assembly)
        {
            CommandManifest findInType = _finder.FindInAssembly(assembly);

            if (_manifest == null)
                _manifest = findInType;
            else
                _manifest.Commands.AddRange(findInType.Commands);

            return this;
        }

        public void Run(string[] args)
        {
            if (_manifest == null)
                _manifest = _finder.FindInAssembly(Assembly.GetCallingAssembly());

            if(_help == null)
                _help = CommandLineHelpGenerator.Generate(_manifest);

            if (args == null || args.Length == 0)
            {
                ShowHelp();
                return;
            }

            ICommandLineParser parser = new CommandLineParser();
            var parseResult = parser.Parse(_manifest, args);

            try
            {
                string commandName = CommandNameExtractor.Extract(parseResult.Values);
                if (commandName == null)
                    throw new CommandException("Cannot derive command name from input.");

                var command = new CommandResolver().Resolve(_manifest, commandName);

                if (command == null)
                    throw new CommandException(String.Format("There is no command with name '{0}'.", commandName));

                var commandParseResult = parseResult[command];
                
                if(!commandParseResult.WasSuccessfullyParsed)
                    throw new CommandException(commandParseResult.Message);
                
                command.Run(parseResult[command]);
            }
            catch (CommandException commandException)
            {
                ShowErrorMessage(commandException);
                ShowHelp();
            }
            catch(TargetInvocationException targetInvocationException)
            {
                if (targetInvocationException.InnerException != null)
                    throw targetInvocationException.InnerException;
                throw;
            }
        }

        private void ShowErrorMessage(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.ResetColor();
        }

        private void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Usage: {0} <command> [options]", Process.GetCurrentProcess().ProcessName);
            Console.WriteLine();

            foreach (var command in _help.Commands)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(command.FormattedLine);
                Console.ResetColor();
                foreach (var parameter in command.Parameters)
                {
                    Console.WriteLine(parameter.FormattedLine);
                }

                Console.WriteLine();
            }
        }
    }
}