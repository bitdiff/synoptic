using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;
using Synoptic.ConsoleUtilities;
using Synoptic.HelpUtilities;

namespace Synoptic
{
    public class CommandRunner
    {
        private readonly TextWriter _error = Console.Error;

        private readonly List<Command> _availableCommands = new List<Command>();
        private readonly ICommandActionFinder _actionFinder = new CommandActionFinder();
        private readonly CommandFinder _commandFinder = new CommandFinder();
        private IDependencyResolver _resolver = new ActivatorDependencyResolver();
        private CommandLineHelp _help;
        private OptionSet _optionSet;

        public void Run(string[] args)
        {
            List<string> arguments = new List<string>(args);

            if (_optionSet != null)
            {
                arguments = _optionSet.Parse(arguments);
            }

            if (_availableCommands.Count == 0)
                WithCommandsFromAssembly(Assembly.GetCallingAssembly());

            if (_availableCommands.Count == 0)
            {
                Out.WordWrap("There are currently no commands defined.\nPlease ensure commands are correctly defined and registered within Synoptic using the [Command] attribute.");
                return;
            }

            if (arguments.Count == 0)
            {
                new CommandLineHelpGenerator().ShowCommandHelp(_availableCommands, _optionSet);
                return;
            }

            try
            {
                var firstArg = arguments.First();
                arguments.RemoveAt(0);
                args = arguments.ToArray();
                var commandSelector = new CommandSelector();
                var command = commandSelector.Select(firstArg, _availableCommands);

                var parser = new CommandLineParser();

                CommandLineParseResult parseResult = parser.Parse(command, args);
                _help = new CommandLineHelp(new[] { parseResult.CommandAction });
                if (!parseResult.WasSuccessfullyParsed)
                    throw new CommandActionException(parseResult.Message);

                parseResult.CommandAction.Run(_resolver, parseResult);
            }
            catch (CommandActionException commandException)
            {
                ShowErrorMessage(commandException);
            }
            catch (TargetInvocationException targetInvocationException)
            {
                Exception innerException = targetInvocationException.InnerException;

                if (innerException == null) throw;

                if (innerException is CommandActionException)
                {
                    ShowErrorMessage(innerException);
                }

                throw new CommandInvocationException("Error executing command", innerException);
            }
        }

        private void ShowErrorMessage(Exception exception)
        {
            _error.WriteLine(exception.Message);
        }


        public CommandRunner WithDependencyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
            return this;
        }

        public CommandRunner WithCommandsFromType<T>()
        {
            _availableCommands.Add(_commandFinder.FindInType(typeof(T)));
            return this;
        }

        public CommandRunner WithCommandsFromAssembly(Assembly assembly)
        {
            _availableCommands.AddRange(_commandFinder.FindInAssembly(assembly));
            return this;
        }

        public CommandRunner WithGlobalOptions(OptionSet optionSet)
        {
            _optionSet = optionSet;
            return this;
        }
    }
}