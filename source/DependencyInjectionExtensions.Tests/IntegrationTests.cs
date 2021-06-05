using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionExtensions.Tests.Decorator;
using DependencyInjectionExtensions.Tests.HostedService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nricher.DependencyInjectionExtensions;
using Nricher.DependencyInjectionExtensions.AddImplementation;
using Nricher.DependencyInjectionExtensions.Decorator;
using Nricher.DependencyInjectionExtensions.HostedService;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests
{
    public class IntegrationTests
    {
        [Test]
        public void DecoratedAndHostedTest()
        {
            GivenServiceCollectionExtensionWithDecoratorAndHosted();
            WhenAddService();
            ThenServiceDecoratedAndHostedAdded();
        }

        [Test]
        public void ImplementationAndHostedTest()
        {
            GivenServiceCollectionExtensionWithImplementationAndHosted();
            WhenAddService();
            ThenServiceImplementationAddedAndHostedAdded();
        }

        [Test]
        public void DecoratedAndImplementationTest()
        {
            GivenServiceCollectionExtensionWithDecoratorAndImplementation();
            WhenAddService();
            ThenServiceDecoratedAndImplementationAdded();
        }

        [Test]
        public void DecoratedImplementationAndHostedTest()
        {
            GivenServiceCollectionExtensionWithDecoratorImplementationAndHosted();
            WhenAddService();
            ThenServiceDecoratedImplementationAddedAndHostedAdded();
        }

        private IServiceCollection _serviceCollection;

        private void GivenServiceCollectionExtensionWithDecoratorAndHosted()
        {
            CreateServiceCollection(new IServiceCollectionExtension[]
            {
                new HostedServiceExtension(),
                new DecoratorExtension(new DecoratorOneFactory())
            });
        }

        private void GivenServiceCollectionExtensionWithImplementationAndHosted()
        {
            CreateServiceCollection(new IServiceCollectionExtension[]
            {
                new HostedServiceExtension(),
                new NotTransientImplementationExtension()
            });
        }

        private void GivenServiceCollectionExtensionWithDecoratorAndImplementation()
        {
            CreateServiceCollection(new IServiceCollectionExtension[]
            {
                new NotTransientImplementationExtension(),
                new DecoratorExtension(new DecoratorOneFactory())
            });
        }

        private void GivenServiceCollectionExtensionWithDecoratorImplementationAndHosted()
        {
            CreateServiceCollection(new IServiceCollectionExtension[]
            {
                new HostedServiceExtension(),
                new NotTransientImplementationExtension(),
                new DecoratorExtension(new DecoratorOneFactory())
            });
        }

        private void WhenAddService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, HostedObjectUnderTest>();
        }

        private void ThenServiceDecoratedAndHostedAdded()
        {
            var provider = _serviceCollection.BuildServiceProvider();

            AssertHasHosted(provider);
            AssertIsDecorated(provider);
        }

        private void ThenServiceImplementationAddedAndHostedAdded()
        {
            var provider = _serviceCollection.BuildServiceProvider();

            AssertHasHosted(provider);
            AssertImplementationAdded(provider);
        }

        private void ThenServiceDecoratedAndImplementationAdded()
        {
            var provider = _serviceCollection.BuildServiceProvider();

            AssertImplementationAdded(provider);
            AssertIsDecorated(provider);
        }

        private void ThenServiceDecoratedImplementationAddedAndHostedAdded()
        {
            var provider = _serviceCollection.BuildServiceProvider();

            AssertHasHosted(provider);
            AssertImplementationAdded(provider);
            AssertIsDecorated(provider);
        }

        private void CreateServiceCollection(IEnumerable<IServiceCollectionExtension> extensions)
        {
            _serviceCollection = new ServiceCollectionExtender(new ServiceCollection(), extensions);
        }

        private static void AssertHasHosted(IServiceProvider provider)
        {
            Assert.That(provider.GetServices<IHostedService>().Count(), Is.EqualTo(1));
        }

        private static void AssertImplementationAdded(IServiceProvider provider)
        {
            Assert.IsNotNull(provider.GetService<HostedObjectUnderTest>());
        }

        private static void AssertIsDecorated(IServiceProvider provider)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var interfaceEnsurerDecorator = (InterfaceEnsurerDecorator)provider.GetService<IObjectUnderTest>();
            Assert.IsInstanceOf<DecoratorOne<IObjectUnderTest>>(interfaceEnsurerDecorator?.Decorated);
        }
    }
}
