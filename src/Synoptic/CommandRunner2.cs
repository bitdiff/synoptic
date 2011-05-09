using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private IMiddleware<Request, Response>[] _middleware;

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
                var pipeline = new Pipeline<Request, Response>();
                foreach (var m in _middleware)
                {
                    pipeline.Add(m);

                }

                var response = pipeline.Execute(new Request(args));
                response.Execute(false);

                var commandSelector = new CommandSelector(_commandManifest.Commands);
                var command = commandSelector.Select(ref args);

                var parser = new CommandLineParser2();

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
        public CommandRunner2 WithMiddleware(params IMiddleware<Request, Response>[] middleware)
        {
            _middleware = middleware;
            return this;
        }
    }

    public interface IMiddleware<T, TOut>
    {
        TOut Process(T context, Func<T, TOut> executeNext);
    }

    public class Pipeline<T, TOut>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<IMiddleware<T, TOut>> _filters = new List<IMiddleware<T, TOut>>();
        private int _current;

        public Pipeline()
        //            : this(new ActivatorServiceProvider())
        { }

        public Pipeline<T, TOut> Add(Type filterType)
        {
            Add((IMiddleware<T, TOut>)_serviceProvider.GetService(filterType));
            return this;
        }

        public Pipeline<T, TOut> Add<TFilter>() where TFilter : IMiddleware<T, TOut>
        {
            Add(typeof(TFilter));
            return this;
        }

        public int Count
        {
            get { return _filters.Count; }
        }

        public Pipeline<T, TOut> Add(IMiddleware<T, TOut> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public TOut Execute(T input)
        {
            GetNext = () => _current < _filters.Count
                ? x => _filters[_current++].Process(x, GetNext())
                : new Func<T, TOut>(c => { throw new EndOfChainException(); });

            return GetNext().Invoke(input);
        }

        private Func<Func<T, TOut>> GetNext { get; set; }
    }

    public class EndOfChainException : Exception
    {
        public EndOfChainException()
            : base("Next filter does not exist."
            + " Use 'Finally' or a filter that short-circuits before reaching the end of the chain")
        { }
    }

    public static class AssemblyExtensions
    {
        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(this Pipeline<T, TOut> pipeline, Assembly assembly)
        {
            return AddFiltersIn(pipeline, Assembly.GetCallingAssembly(), t => true);
        }

        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(this Pipeline<T, TOut> pipeline, string filterNamespace)
        {
            return AddFiltersIn(pipeline, Assembly.GetCallingAssembly(), filterNamespace);
        }

        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(
            this Pipeline<T, TOut> pipeline,
            Assembly assembly,
            string filterNamespace)
        {
            return AddFiltersIn(pipeline, assembly, t => t.Namespace == filterNamespace);
        }

        public static Pipeline<T, TOut> AddFiltersIn<T, TOut>(
            this Pipeline<T, TOut> pipeline,
            Assembly assembly,
            Func<Type, bool> predicate)
        {
            var filterTypes = assembly
                .GetTypes()
                .Where(t => typeof(IMiddleware<T, TOut>).IsAssignableFrom(t));

            foreach (var filterType in filterTypes.Where(predicate))
            {
                pipeline.Add(filterType);
            }

            return pipeline;
        }
    }

    public static class PipelineExtensions
    {
        /// <summary>
        /// Convenience method to add a short-circuiting filter to the end of the chain
        /// </summary>
        public static Pipeline<T, TOut> Finally<T, TOut>(this Pipeline<T, TOut> pipeline, Func<T, TOut> func)
        {
            pipeline.Add(new ShortCircuit<T, TOut>(func));
            return pipeline;
        }

        private class ShortCircuit<T, TOut> : IMiddleware<T, TOut>
        {
            private readonly Func<T, TOut> _func;

            public ShortCircuit(Func<T, TOut> func)
            {
                this._func = func;
            }

            public TOut Process(T context, Func<T, TOut> executeNext)
            {
                return _func.Invoke(context);
            }
        }
    }
}