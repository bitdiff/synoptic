using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private Func<string[], string[]> _preProcessor;
        private IMiddleware<Request, Response>[] _middleware;

        public CommandRunner WithDependencyResolver(IDependencyResolver resolver)
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
        public CommandRunner WithCommandsFromAssembly(Assembly assembly)
        {
            _commands.AddRange(_commandFinder.FindInAssembly(assembly));
            return this;
        }

        public void Run(string[] args)
        {
            var arguments = new List<string>(args);

            if (_commands.Count == 0)
                WithCommandsFromAssembly(Assembly.GetCallingAssembly());

            if (_commands.Count == 0)
            {
                _error.WriteLine("There are currently no commands defined.\nPlease ensure commands are correctly defined and registered within Synoptic.");
                return;
            }

            //            if (_help == null)
            //                _help = CommandLineHelpGenerator.Generate(_commandManifest);

            if (arguments.Count == 0)
            {
                ShowCommands();
                return;
            }

            try
            {
                var pipeline = new Pipeline<Request, Response>();
                foreach (var m in _middleware)
                {
                    pipeline.Add(m);

                }

                var response = pipeline.Execute(new Request(arguments.ToArray()));
                response.Execute(false);

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
            _error.WriteLine("Usage: {0} <command> [options]", Process.GetCurrentProcess().ProcessName);
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

        private void ShowCommands()
        {
            _error.WriteLine();
            _error.WriteLine("Usage: {0} <command> [options]", Process.GetCurrentProcess().ProcessName);
            _error.WriteLine();

            foreach (var command in _commands)
            {
                _error.WriteLine(command.Name + "    " + command.Description);
                _error.WriteLine();
            }
        }
        
        public CommandRunner WithMiddleware(params IMiddleware<Request, Response>[] middleware)
        {
            _middleware = middleware;
            return this;
        }
    }
}