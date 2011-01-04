using System;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Synoptic.Service
{
    public sealed class WindowsService : ServiceBase
    {
        private const string LoggerName = "WindowsService";
        private readonly IDaemon _daemon;
        private readonly ILogger _logger;
        private readonly IWindowsServiceConfiguration _configuration;

        public WindowsService(IDaemon daemon, ILogger logger, IWindowsServiceConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _daemon = daemon;
            _logger = logger;
            _configuration = configuration;

            EventLog.Log = "Application";
            ServiceName = _configuration.ServiceName;
            CanStop = true;
            CanShutdown = true;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _logger.LogInfo(LoggerName, "starting service {0}", ServiceName);
            try
            {
                _daemon.Start();
            }
            catch (Exception e)
            {
                _logger.LogException(LoggerName, e, "starting service {0}", ServiceName);
                throw;
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            _logger.LogInfo(LoggerName, "stopping service {0}", ServiceName);
            try
            {
                _daemon.Stop();
            }
            catch (Exception e)
            {
                _logger.LogException(LoggerName, e, "error stopping server");
                throw;
            }
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _logger.LogInfo(LoggerName, "system is shuttingdown.. stopping service {0}", ServiceName);
            try
            {
                _daemon.Stop();
            }
            catch (Exception e)
            {
                _logger.LogException(LoggerName, e, "error stopping server during shutdown");
                throw;
            }
        }

        public bool IsInstalled()
        {
            return ServiceController.GetServices().Any(service => service.ServiceName == ServiceName);
        }

        public void Install()
        {
            _logger.LogInfo(LoggerName, "installing service {0}", ServiceName);
            using (var installer = new TransactedInstaller())
            {
                SetInstallers(installer);
                // There is a bug in .NET 3.5 where the image path will not be escaped correctly.
                installer.Context = new InstallContext(null, new[] { "/assemblypath=\"" + Process.GetCurrentProcess().MainModule.FileName + "\" " + _configuration.CommandLineArguments });
                installer.AfterInstall += ModifyImagePath;
                installer.Install(new Hashtable());
            }
        }

        private void ModifyImagePath(object sender, InstallEventArgs e)
        {
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            string path = string.Format("\"{0}\" {1}", exe, _configuration.CommandLineArguments);

            _logger.LogInfo(LoggerName, "patching registry for service {0}", ServiceName);
            
            Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services")
                .OpenSubKey(_configuration.ServiceName, true)
                .SetValue("ImagePath", path);
        }

        public void Uninstall()
        {
            _logger.LogInfo(LoggerName, "uninstalling service {0}", ServiceName);
            using (var installer = new TransactedInstaller())
            {
                SetInstallers(installer);
                installer.Uninstall(null);
            }
        }

        private void SetInstallers(Installer installer)
        {
            installer.Installers.Add(new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem });
            installer.Installers.Add(new ServiceInstaller { DisplayName = _configuration.DisplayName, Description = _configuration.Description, ServiceName = _configuration.ServiceName, StartType = ServiceStartMode.Automatic });
        }
    }
}