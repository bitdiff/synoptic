using System;
using System.ServiceProcess;
using System.Threading;

namespace Synoptic.Service.Demo
{
    class CommandSet
    {
        private readonly ILogger _logger;
        private readonly IDaemon _daemon;
        private readonly WindowsService _service;
        private readonly ManualResetEvent _resetEvent;

        public CommandSet()
        {
            _resetEvent = new ManualResetEvent(false);
            _logger = new SimpleLogger();
            _daemon = new MyDaemon(_logger, _resetEvent);
            var config = new WindowsServiceConfiguration("aa1") { CommandLineArguments = "windows-service --start" };
            _service = new WindowsService(_daemon, _logger, config);
        }

        [Command]
        public void WindowsService(bool start, bool install, bool remove)
        {
            if (start)
                ServiceBase.Run(_service);
            else if (install)
                _service.Install();
            else if (remove)
                _service.Uninstall();
            else
                throw new CommandException("You must specify at least one option");
        }

        [Command]
        public void StartAsConsoleService()
        {
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; _resetEvent.Set(); };
            _daemon.Start();
            _resetEvent.WaitOne();
            _daemon.Stop();
        }
    }
}