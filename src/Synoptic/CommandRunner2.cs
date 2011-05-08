using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Synoptic.HelpUtilities;

namespace Synoptic
{
    public class CommandRunner2
    {
        private readonly TextWriter _error = Console.Error;

        private readonly CommandManifest _commandManifest = new CommandManifest();
        private readonly ICommandActionFinder _actionFinder = new CommandActionActionFinder();
        private readonly CommandFinder _commandFinder = new CommandFinder();
        private IDependencyResolver _resolver = new ActivatorDependencyResolver();
        private CommandLineHelp _help;
        private Func<CommandLineParseResult, object> _commandSetInstantiator;
        private Func<string[], string[]> _preProcessor;

        public CommandRunner2 WithDependencyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
            return this;
        }

//        public CommandRunner2 WithCommandsFromType<T>()
//        {
//            _commandManifest.Commands.AddRange(_actionFinder.FindInType(typeof(T)).Commands);
//            return this;
//        }
//
        public CommandRunner2 WithCommandsFromAssembly(Assembly assembly)
        {
            _commandManifest.Commands.AddRange(_commandFinder.FindInAssembly(assembly).Commands);
            return this;
        }

        public void Run(string[] args)
        {
            if (_commandManifest.Commands.Count == 0)
                WithCommandsFromAssembly(Assembly.GetCallingAssembly());

            if (_commandManifest.Commands.Count == 0)
            {
                _error.WriteLine("There are currently no commands defined.\nPlease ensure commands are correctly defined and registered within Synoptic.");
                return;
            }

//            if (_help == null)
//                _help = CommandLineHelpGenerator.Generate(_commandManifest);

            if (args == null || args.Length == 0)
            {
                ShowCommands();
                return;
            }

            try
            {
                var commandSelector = new CommandSelector(_commandManifest.Commands);
                var command = commandSelector.Select(ref args);

                var parser = new CommandLineParser2();

                CommandLineParseResult parseResult =  parser.Parse(command, args);
                _help = new CommandLineHelp(new[] { parseResult.CommandAction});
                if (!parseResult.WasSuccessfullyParsed)
                    throw new CommandActionException(parseResult.Message);

                if (_commandSetInstantiator != null)
                    parseResult.CommandAction.Run(_commandSetInstantiator(parseResult), parseResult);
                else
                    parseResult.CommandAction.Run(_resolver, parseResult);
            }
            catch (CommandActionException commandException)
            {
                ShowErrorMessage(commandException);
                ShowHelp();
            }
            catch (TargetInvocationException targetInvocationException)
            {
                Exception innerException = targetInvocationException.InnerException;

                if (innerException == null) throw;

                if (innerException is CommandActionException)
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

        private void ShowCommands()
        {
            _error.WriteLine();
            _error.WriteLine("Usage: {0} <command> [options]", Process.GetCurrentProcess().ProcessName);
            _error.WriteLine();

            foreach (var command in _commandManifest.Commands)
            {
                _error.WriteLine(command.Name + "    " + command.Description);
                _error.WriteLine();
            }
        }

//        public CommandRunner2 WithCommandSet<T>(Func<CommandLineParseResult, object> commandSetInstantiator)
//        {
//            _commandManifest.Commands.AddRange(_actionFinder.FindInType(typeof(T)).Commands);
//            _commandSetInstantiator = commandSetInstantiator;
//
//            return this;
//        }
//
//        public CommandRunner2 WithArgsPreProcessor(Func<string[], string[]> preProcessor)
//        {
//            _preProcessor = preProcessor;
//            return this;
//        }
    }
}