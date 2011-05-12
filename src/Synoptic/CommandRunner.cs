using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly List<Command> _commands = new List<Command>();
        private readonly ICommandActionFinder _actionFinder = new CommandActionFinder();
        private readonly CommandFinder _commandFinder = new CommandFinder();
        private IDependencyResolver _resolver = new ActivatorDependencyResolver();
        private CommandLineHelp _help;
        private OptionSet _optionSet;

        public void Run(string[] args)
        {
            if (_optionSet != null)
            {
                _optionSet.WriteOptionDescriptions(Console.Out);
                args = _optionSet.Parse(args).ToArray();
            }

            var arguments = new List<string>(args);

            if (_commands.Count == 0)
                WithCommandsFromAssembly(Assembly.GetCallingAssembly());

            if (_commands.Count == 0)
            {
                _error.WriteLine("There are currently no commands defined.\nPlease ensure commands are correctly defined and registered within Synoptic.");
                return;
            }

            if (arguments.Count == 0)
            {
                ShowCommandHelp();
                return;
            }

            try
            {
                var firstArg = arguments.First();
                arguments.RemoveAt(0);
                args = arguments.ToArray();
                var commandSelector = new CommandSelector();
                var command = commandSelector.Select(firstArg, _commands);

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
            _error.WriteLine("Usage: {0} <command> <action> [options]", Process.GetCurrentProcess().ProcessName);
            _error.WriteLine();

            _error.WriteLine("Help goes here...");
            //            foreach (var command in _help.Commands)
            //            {
            //                _error.WriteLine(command.FormattedLine);
            //                foreach (var parameter in command.Parameters)
            //                {
            //                    _error.WriteLine(parameter.FormattedLine);
            //                }
            //
            //                _error.WriteLine();
            //            }
        }

        private void ShowCommandHelp()
        {
            _error.WriteLine("Usage: {0} COMMAND ACTION [ARGS]", Process.GetCurrentProcess().ProcessName);
            _error.WriteLine();

            int SpacingWidth = 3;

            var spacer = new string(' ', SpacingWidth);
            var maximumCommandNameLength = _commands.Count() > 0 ? _commands.Max(c => c.Name.Length) : 0;

            Out.WordWrap("The available commands are:\n");

            foreach (var command in _commands)
            {
                Out.WordWrap(String.Format("{0," + -maximumCommandNameLength + "}{1}", command.Name, spacer), spacer.Length);
                Out.WordWrap(String.Format("{0}\n", command.Description), Console.CursorLeft);
                //                _error.WriteLine(command.Name + "    " + command.Description);
                //                _error.WriteLine();
            }
        }

        public CommandRunner WithDependencyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
            return this;
        }

        public CommandRunner WithCommandsFromType<T>()
        {
            _commands.Add(_commandFinder.FindInType(typeof(T)));
            return this;
        }

        public CommandRunner WithCommandsFromAssembly(Assembly assembly)
        {
            _commands.AddRange(_commandFinder.FindInAssembly(assembly));
            return this;
        }

        public CommandRunner WithGlobalOptions(OptionSet optionSet)
        {
            _optionSet = optionSet;
            return this;
        }
    }
}