using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
                ShowCommandHelp(_optionSet);
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

        private void ShowCommandHelp(OptionSet optionSet)
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var usagePreamble = String.Format("Usage: {0} ", processName);
            Out.WordWrap(usagePreamble);
            
            var usagePattern = GetUsagePattern(optionSet);
            Out.WordWrap(usagePattern, usagePreamble.Length);
            Console.WriteLine("\n");

            const int spacingWidth = 3;

            var spacer = new string(' ', spacingWidth);
            var maximumCommandNameLength = _availableCommands.Count() > 0 ? _availableCommands.Max(c => c.Name.Length) : 0;

            Out.WordWrap("The available commands are:\n");

            foreach (var command in _availableCommands)
            {
                Out.WordWrap(String.Format("{0," + -maximumCommandNameLength + "}{1}", command.Name, spacer), spacer.Length);
                Out.WordWrap(String.Format("{0}\n", command.Description), Console.CursorLeft);
            }
        }

        public CommandRunner WithDependencyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
            return this;
        }

        public string GetUsagePattern(OptionSet optionSet)
        {
            var output = new StringBuilder();

            // Generate usage.
            if (optionSet != null)
            {
                foreach (Option o in _optionSet)
                {
                    if (o.OptionValueType == OptionValueType.None)
                    {
                        output.AppendFormat("[--{0}]", o.Prototype);
                    }
                    if (o.OptionValueType == OptionValueType.Required)
                    {
                        output.AppendFormat("[--{0}VALUE]", o.Prototype);
                    }

                    output.Append(" ");
                }
            }

            return output.Append("COMMAND ACTION [ARGS]").ToString();
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