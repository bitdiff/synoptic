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
        public event EventHandler<WindowsServiceEventArgs> Starting = (s, e) => { };
        public event EventHandler<WindowsServiceEventArgs> Started = (s, e) => { };
        public event EventHandler<WindowsServiceEventArgs> Stopping = (s, e) => { };
        public event EventHandler<WindowsServiceEventArgs> Stopped = (s, e) => { };
        public event EventHandler<ErrorEventArgs> Error = (s, e) => { };

        private void OnEvent(EventHandler<WindowsServiceEventArgs> handle, WindowsServiceEventArgs e)
        {
            if (handle != null)
                handle(this, e);
        }

        private void OnEvent(EventHandler<ErrorEventArgs> handle, ErrorEventArgs e)
        {
            if (handle != null)
                handle(this, e);
        }

        private readonly IServiceDaemon _daemon;
        private readonly IWindowsServiceConfiguration _configuration;

        public WindowsService(string serviceName, IServiceDaemon daemon, Action<WindowsServiceConfiguration> configure) :
            this(daemon, SetConfiguration(serviceName, configure))
        {
        }

        private static IWindowsServiceConfiguration SetConfiguration(string serviceName, Action<WindowsServiceConfiguration> configure)
        {
            var configuration = new WindowsServiceConfiguration(serviceName);
            configure(configuration);

            return configuration;
        }

        public WindowsService(string serviceName, IServiceDaemon daemon)
            : this(daemon, new WindowsServiceConfiguration(serviceName))
        {
        }

        public WindowsService(IServiceDaemon daemon, IWindowsServiceConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (configuration.ServiceName == null)
                throw new ArgumentNullException("configuration.ServiceName");

            _daemon = daemon;
            _configuration = configuration;

            EventLog.Log = "Application";
            ServiceName = _configuration.ServiceName;
            CanStop = true;
            CanShutdown = true;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            OnEvent(Starting, new WindowsServiceEventArgs(ServiceName));

            try
            {
                _daemon.Start();
            }
            catch (Exception e)
            {
                OnEvent(Error, new ErrorEventArgs(new DaemonException(String.Format("Error starting service '{0}'.", ServiceName), e)));
                throw;
            }

            OnEvent(Started, new WindowsServiceEventArgs(ServiceName));
        }

        protected override void OnStop()
        {
            base.OnStop();

            OnEvent(Stopping, new WindowsServiceEventArgs(ServiceName));

            try
            {
                _daemon.Stop();
            }
            catch (Exception e)
            {
                OnEvent(Error, new ErrorEventArgs(new DaemonException(String.Format("Error stopping service '{0}'.", ServiceName), e)));
                throw;
            }

            OnEvent(Stopped, new WindowsServiceEventArgs(ServiceName));
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _daemon.Stop();
        }

        public bool IsInstalled()
        {
            return ServiceController.GetServices().Any(service => service.ServiceName == ServiceName);
        }

        public void Install()
        {
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

            RegistryKey key = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services");
            if (key != null)
            {
                RegistryKey subKey = key.OpenSubKey(_configuration.ServiceName, true);
                if (subKey != null)
                    subKey.SetValue("ImagePath", path);
            }
        }

        public void Uninstall()
        {
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