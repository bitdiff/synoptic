using Mono.Options;
using StructureMap;
using Synoptic.Demo.Services;

namespace Synoptic.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectFactory.Initialize(service =>
            {
                service.For<IMyService>().Use<MyService>();
                service.For<IMyService2>().Use<MyService2>();
            });

            var resolver = new StructureMapDependencyResolver(ObjectFactory.Container);

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
