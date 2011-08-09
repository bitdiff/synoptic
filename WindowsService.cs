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
        private readonly IDaemon _daemon;
        private readonly IWindowsServiceConfiguration _configuration;

        public WindowsService(IDaemon daemon, IWindowsServiceConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (configuration.ServiceName == null)
                throw new ArgumentNullException("configuration", "ServiceName cannot be null");

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
            _daemon.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _daemon.Stop();
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

        public void Run()
        {
            Run(this);
        }
    }

    public interface IWindowsServiceConfiguration
    {
        string ServiceName { get; }
        string CommandLineArguments { get; }
        string Description { get; }
        string DisplayName { get; }
    }

    public class WindowsServiceConfiguration : IWindowsServiceConfiguration
    {
        private readonly string _serviceName;

        public WindowsServiceConfiguration(string serviceName)
        {
            if (String.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("serviceName");

            _serviceName = serviceName;
        }

        public string ServiceName
        {
            get { return _serviceName; }
        }

        public string CommandLineArguments { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
    }

    public interface IDaemon
    {
        void Start();
        void Stop();
    }
}