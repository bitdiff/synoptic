using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Synoptic.HelpUtilities;

namespace Synoptic
{
    public class CommandRunner
    {
        private readonly TextWriter _error = Console.Error;

        private readonly CommandManifest _manifest = new CommandManifest();
        private readonly ICommandFinder _finder;
        private IDependencyResolver _resolver = new ActivatorDependencyResolver();
        private CommandLineHelp _help;

        public CommandRunner()
        {
            _finder = new CommandFinder();
        }

        public CommandRunner WithDependencyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
            return this;
        }

        public CommandRunner WithCommandsFromType<T>()
        {
            _manifest.Commands.AddRange(_finder.FindInType(typeof(T)).Commands);
            return this;
        }

        public CommandRunner WithCommandsFromAssembly(Assembly assembly)
        {
            _manifest.Commands.AddRange(_finder.FindInAssembly(assembly).Commands);
            return this;
        }

        public void Run(string[] args)
        {
            if (_manifest.Commands.Count == 0)
                WithCommandsFromAssembly(Assembly.GetCallingAssembly());

            if (_manifest.Commands.Count == 0)
            {
                _error.WriteLine("There are currently no commands defined.\nPlease ensure commands are correctly defined and registered within Synoptic.");
                return;
            }

            if (_help == null)
                _help = CommandLineHelpGenerator.Generate(_manifest);

            if (args == null || args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                ICommandLineParser parser = new CommandLineParser();
                CommandLineParseResult parseResult = parser.Parse(_manifest, args);

                if (!parseResult.WasSuccessfullyParsed)
                    throw new CommandException(parseResult.Message);

                parseResult.Command.Run(_resolver, parseResult);
            }
            catch (CommandException commandException)
            {
                ShowErrorMessage(commandException);
                ShowHelp();
            }
            catch (TargetInvocationException targetInvocationException)
            {
                Exception innerException = targetInvocationException.InnerException;

                if (innerException == null) throw;

                if (innerException is CommandException)
                {
                    ShowErrorMessage(innerException);
                    ShowHelp();
                }

                throw new CommandInvocationException("Error executing command", innerException);
            }
        }

        private void ShowErrorMessage(Exception exception)
        {
            _error.WriteLine(exception.Message);
        }

        private void ShowHelp()
        {
            _error.WriteLine();
            _error.WriteLine("Usage: {0} <command> [options]", Process.GetCurrentProcess().ProcessName);
            _error.WriteLine();

            foreach (var command in _help.Commands)
            {
                _error.WriteLine(command.FormattedLine);
                foreach (var parameter in command.Parameters)
                {
                    _error.WriteLine(parameter.FormattedLine);
                }

                _error.WriteLine();
            }
        }
    }
}