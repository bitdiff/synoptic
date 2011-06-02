using System;
using Mono.Options;
using Ninject;
using StructureMap;
using Synoptic.ConsoleFormat;
using Synoptic.Demo.Services;

namespace Synoptic.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
//            var consoleTable = new ConsoleTable(
//
//                new ConsoleRow(
//                    new ConsoleCell(new string('A', 40)).WithPadding(0).WithWidth(40), new ConsoleCell("Lorem ipsum dolor sit amet cras amet Lorem ipsum dolor sit amet cras amet. Lorem ipsum dolor sit amet cras amet.").WithPadding(3).WithWidth(40)), 
//                    new ConsoleRow(new ConsoleCell(new string('Y', 80)).WithPadding(0)));
//
//            ConsoleFormatter.Write(consoleTable);
//            
//            return;

//            ObjectFactory.Initialize(service =>
//            {
//                service.For<IMyService>().Use<MyService>();
//                service.For<IMyService2>().Use<MyService2>();
//            });

            var c = new Container(service =>
            {
                service.For<IMyService>().Use<MyService>();
                service.For<IMyService2>().Use<MyService2>();
            });

            var k = new StandardKernel();
            k.Bind<IMyService>().To<MyService>();
            k.Bind<IMyService2>().To<MyService2>();

            //var resolver = new StructureMapDependencyResolver(c);
            var resolver = new NinjectDependencyResolver(k);

            var optionSet = new OptionSet
                                {
                                    { "h|help", "shows help",v => { GlobalOptions.Help = v; } },
                                    { "m|master=", v => { GlobalOptions.Help = v; } },
                                    { "s=", v => { GlobalOptions.Help = v; } },
                                    { "lll|llllll=", v => { GlobalOptions.Help = v; } },
                                };
            
            new CommandRunner()
                .WithDependencyResolver(resolver)
                .WithGlobalOptions(optionSet)
                .Run(args);
        }

        public static MyGlobalOptions GlobalOptions = new MyGlobalOptions();
    }
}
