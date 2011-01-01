using System;
using StructureMap;

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

            new CommandRunner()
                .WithDependencyResolver(resolver)
                .Run(args);
        }
    }

    internal class MyService : IMyService
    {
        public string Hello(string message)
        {
            return "Hello " + message;
        }
    }

    internal interface IMyService
    {
        String Hello(string message);
    }

    internal class MyService2 : IMyService2
    {
        public string Hello2(string message)
        {
            return "Hello2 " + message;
        }
    }

    internal interface IMyService2
    {
        String Hello2(string message);
    }

    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public StructureMapDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object Resolve(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}
