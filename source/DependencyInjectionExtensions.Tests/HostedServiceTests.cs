using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests
{
    public class HostedServiceTests
    {
        [Test]
        public void RegisterAsHostedServiceTest()
        {
            GivenCollection();
            WhenRegisterImplementationService();
            ThenHostedServiceRegistered();
        }

        [Test]
        public void RegisterServiceTest()
        {
            GivenCollection();
            WhenRegisterService();
            ThenServiceAndHostedServiceAreSame<IObjectUnderTest>();
        }

        [Test]
        public void RegisterInstanceTest()
        {
            GivenCollection();
            WhenRegisterInstance();
            ThenHostedServiceRegistered();
        }

        [Test]
        public void RegisterInstanceAsServiceTest()
        {
            GivenCollection();
            WhenRegisterInstanceAsService();
            ThenHostedServiceRegistered();
            ThenServiceAndHostedServiceAreSame<IObjectUnderTest>();
        }

        [Test]
        public void RegisterImplementationFactoryTest()
        {
            GivenCollection();
            WhenRegisterImplementationFactory();
            ThenHostedServiceRegistered();
        }

        [Test]
        public void RegisterImplementationFactoryAsServiceTest()
        {
            GivenCollection();
            WhenRegisterImplementationFactoryAsService();
            ThenHostedServiceRegistered();
            ThenServiceAndHostedServiceAreSame<IObjectUnderTest>();
        }

        [Test]
        public void RegisterHostedServiceTest()
        {
            GivenCollection();
            WhenRegisterHostedService();
            ThenHostedServiceRegistered();
            ThenServiceAndHostedServiceAreSame<IHostedServiceUnderTest>();
        }

        private IServiceCollection _serviceCollection;

        private void GivenCollection()
        {
            var decorated = new ServiceCollection();
            var hostedServiceExtension = new HostedServiceExtension();

            _serviceCollection = new ServiceCollectionExtender(decorated, new []
            {
                hostedServiceExtension
            });
        }

        private void WhenRegisterImplementationService()
        {
            _serviceCollection.AddTransient<HostedObjectUnderTest>();
        }

        private void WhenRegisterService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, HostedObjectUnderTest>();
        }

        private void WhenRegisterInstance()
        {
            _serviceCollection.AddSingleton(new HostedObjectUnderTest());
        }

        private void WhenRegisterInstanceAsService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest>(new HostedObjectUnderTest());
        }

        private void WhenRegisterImplementationFactory()
        {
            _serviceCollection.AddSingleton(serviceProvider => new HostedObjectUnderTest());
        }

        private void WhenRegisterImplementationFactoryAsService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, HostedObjectUnderTest>(serviceProvider => new HostedObjectUnderTest());
        }

        private void WhenRegisterHostedService()
        {
            _serviceCollection.AddSingleton<IHostedServiceUnderTest, HostedServiceUnderTest>();
        }

        private void ThenHostedServiceRegistered()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            CollectionAssert.IsNotEmpty(serviceProvider.GetServices<IHostedService>());
            CollectionAssert.AllItemsAreNotNull(serviceProvider.GetServices<IHostedService>());
        }

        private void ThenServiceAndHostedServiceAreSame<T>()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var hostedServices = serviceProvider.GetServices<IHostedService>();
            Assert.AreSame(serviceProvider.GetService<T>(), hostedServices.Single());
        }
    }
}
