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
        private const string LogName = "WindowsService";
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
            _logger.LogInfo(LogName, "Starting service {0}..", ServiceName);
            try
            {
                _daemon.Start();
            }
            catch (Exception e)
            {
                _logger.LogException(LogName, e, "Error starting service {0}", ServiceName);
                throw;
            }
            _logger.LogInfo(LogName, "Started {0}", ServiceName);
        }

        protected override void OnStop()
        {
            base.OnStop();
            _logger.LogInfo(LogName, "Stopping service {0}..", ServiceName);
            try
            {
                _daemon.Stop();
            }
            catch (Exception e)
            {
                _logger.LogException(LogName, e, "Error stopping service {0}", ServiceName);
                throw;
            }
            _logger.LogInfo(LogName, "Stopped service {0}", ServiceName);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _logger.LogInfo(LogName, "System is shuttingdown. Stopping service {0}..", ServiceName);
            try
            {
                _daemon.Stop();
            }
            catch (Exception e)
            {
                _logger.LogException(LogName, e, "Error stopping service {0} during shutdown", ServiceName);
                throw;
            }
            _logger.LogInfo(LogName, "Stopped service {0}", ServiceName);
        }

        public bool IsInstalled()
        {
            return ServiceController.GetServices().Any(service => service.ServiceName == ServiceName);
        }

        public void Install()
        {
            _logger.LogInfo(LogName, "Installing service {0}..", ServiceName);

            using (var installer = new TransactedInstaller())
            {
                SetInstallers(installer);
                // There is a bug in .NET 3.5 where the image path will not be escaped correctly.
                installer.Context = new InstallContext(null, new[] { "/assemblypath=\"" + Process.GetCurrentProcess().MainModule.FileName + "\" " + _configuration.CommandLineArguments });
                installer.AfterInstall += ModifyImagePath;
                installer.Install(new Hashtable());
            }

            _logger.LogInfo(LogName, "Installed service {0}", ServiceName);
        }

        private void ModifyImagePath(object sender, InstallEventArgs e)
        {
            string exe = Process.GetCurrentProcess().MainModule.FileName;
            string path = string.Format("\"{0}\" {1}", exe, _configuration.CommandLineArguments);

            _logger.LogInfo(LogName, "Patching registry for service {0}..", ServiceName);
            
            Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services")
                .OpenSubKey(_configuration.ServiceName, true)
                .SetValue("ImagePath", path);

            _logger.LogInfo(LogName, "Patched registry for service {0}", ServiceName);
        }

        public void Uninstall()
        {
            _logger.LogInfo(LogName, "Uninstalling service {0}..", ServiceName);

            using (var installer = new TransactedInstaller())
            {
                SetInstallers(installer);
                installer.Uninstall(null);
            }

            _logger.LogInfo(LogName, "Uninstalled service {0}", ServiceName);
        }

        private void SetInstallers(Installer installer)
        {
            installer.Installers.Add(new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem });
            installer.Installers.Add(new ServiceInstaller { DisplayName = _configuration.DisplayName, Description = _configuration.Description, ServiceName = _configuration.ServiceName, StartType = ServiceStartMode.Automatic });
        }
    }
}