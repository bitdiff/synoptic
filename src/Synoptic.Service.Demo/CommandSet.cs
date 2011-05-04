using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
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
            _service = new WindowsService("aa1", _daemon, config);
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

        [Command]
        public void RunSkinnyUdpServer()
        {
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; _resetEvent.Set(); };
            var udpServer = new UdpDaemon(message =>
                                              {
                                                  Console.WriteLine("MESSAGE: " + message);
                                                  Thread.Sleep(5000);
                                                  Console.WriteLine("DONE");
                                              },
                                              new UdpDaemonConfiguration(new IPEndPoint(IPAddress.Any, 12345)));
            udpServer.Start();
            _resetEvent.WaitOne();
            udpServer.Stop();
        }

        [Command]
        public void SendUdpMessage(string message)
        {
            var dgram = Encoding.UTF8.GetBytes(message ?? "");

            new UdpClient().Send(dgram, dgram.Length, new IPEndPoint(IPAddress.Broadcast, 12345));
        }
    }
}