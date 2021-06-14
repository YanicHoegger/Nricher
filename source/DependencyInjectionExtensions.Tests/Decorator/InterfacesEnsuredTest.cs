using System.Collections.Generic;
using DependencyInjectionExtensions.Tests.Decorator.InterfaceEnsurer;
using Microsoft.Extensions.DependencyInjection;
using Nricher.DependencyInjectionExtensions;
using Nricher.DependencyInjectionExtensions.Decorator;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class InterfacesEnsuredTest
    {
        [Test]
        public void KeepInterfaceAfterInstanceDecorationTest()
        {
            GivenServiceCollection();
            WhenAddInstance();
            WhenResolveService();
            ThenStillImplementsInterface();
        }

        [Test]
        public void KeepInterfaceAfterImplementationFactoryDecoratedTest()
        {
            GivenServiceCollection();
            WhenAddImplementationFactory();
            WhenResolveService();
            ThenStillImplementsInterface();
        }

        [Test]
        public void KeepInterfaceAfterImplementationTypeDecoratedTest()
        {
            GivenServiceCollection();
            WhenAddImplementationType();
            WhenResolveService();
            ThenStillImplementsInterface();
        }

        [Test]
        public void KeepInterfaceWhenAddInstanceMultipleTimes()
        {
            GivenServiceCollection();
            WhenAddInstance();
            WhenAddInstance();
            WhenResolveServices();
            ThenStillImplementsInterface();
        }

        [Test]
        public void KeepInterfaceWhenAddImplementationFactoryMultipleTimes()
        {
            GivenServiceCollection();
            WhenAddImplementationFactory();
            WhenAddImplementationFactory();
            WhenResolveServices();
            ThenStillImplementsInterface();
        }

        [Test]
        public void KeepInterfaceWhenAddImplementationTypeMultipleTimes()
        {
            GivenServiceCollection();
            WhenAddImplementationType();
            WhenAddImplementationType();
            WhenResolveServices();
            ThenStillImplementsInterface();
        }

        private IServiceCollection _serviceCollection;
        private IEnumerable<IBaseInterface> _services;

        private void GivenServiceCollection()
        {
            _serviceCollection = new ServiceCollectionExtender(new ServiceCollection(), new[] { new DecoratorExtension(new DecoratorOneFactory()) });
        }

        private void WhenAddInstance()
        {
            _serviceCollection.AddSingleton<IBaseInterface>(new Combined());
        }

        private void WhenAddImplementationFactory()
        {
            _serviceCollection.AddSingleton<IBaseInterface>(_ => new Combined());
        }

        private void WhenAddImplementationType()
        {
            _serviceCollection.AddSingleton<IBaseInterface, Combined>();
        }

        private void WhenResolveService()
        {
            _services = new[] { _serviceCollection.BuildServiceProvider().GetService<IBaseInterface>() };
        }

        private void WhenResolveServices()
        {
            _services = _serviceCollection.BuildServiceProvider().GetServices<IBaseInterface>();
        }

        private void ThenStillImplementsInterface()
        {
            foreach (var service in _services)
            {
                Assert.IsTrue(service is IOtherInterface, $"Service does not keep interface {nameof(IOtherInterface)}");
            }
        }
    }
}
