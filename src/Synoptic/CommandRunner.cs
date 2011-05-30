using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Options;
using Synoptic.ConsoleFormat;
using Synoptic.Exceptions;
using Synoptic.HelpUtilities;

namespace Synoptic
{
    public class CommandRunner
    {
        private readonly TextWriter _error = Console.Error;

        private readonly List<Command> _availableCommands = new List<Command>();
        private readonly CommandFinder _commandFinder = new CommandFinder();
        private readonly HelpGenerator _helpGenerator = new HelpGenerator();

        private IDependencyResolver _resolver = new ActivatorDependencyResolver();
        private OptionSet _optionSet;

        public void Run(string[] args)
        {
            Queue<string> arguments = new Queue<string>(args);

            if (_optionSet != null)
                arguments = new Queue<string>(_optionSet.Parse(args));

            if (_availableCommands.Count == 0)
                WithCommandsFromAssembly(Assembly.GetCallingAssembly());

            if (_availableCommands.Count == 0)
                throw new NoCommandsDefinedException();

            if (arguments.Count == 0)
            {
                _helpGenerator.ShowCommandHelp(_availableCommands, _optionSet);
                return;
            }

            try
            {
                var commandName = arguments.Dequeue();
                
                var commandSelector = new CommandSelector();
                var command = commandSelector.Select(commandName, _availableCommands);

                var actionName = arguments.Count > 0 ? arguments.Dequeue() : null;
                
                var actionSelector = new ActionSelector();
                var action = actionSelector.Select(actionName, command);

                var parser = new CommandLineParser();

                CommandLineParseResult parseResult = parser.Parse(action, arguments.ToArray());
                if (!parseResult.WasSuccessfullyParsed)
                    throw new CommandActionException(parseResult.Message);

                parseResult.CommandAction.Run(_resolver, parseResult);
            }
            catch (NoCommandsDefinedException)
            {
                ConsoleFormatter.Write("There are currently no commands defined.\nPlease ensure commands are correctly defined and registered within Synoptic using the [Command] attribute.");
                return;
            }
            catch (CommandNotFoundException exception)
            {
                _helpGenerator.ShowExceptionHelp(exception);
            }
            catch (ActionNotFoundException exception)
            {
                _helpGenerator.ShowExceptionHelp(exception);
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