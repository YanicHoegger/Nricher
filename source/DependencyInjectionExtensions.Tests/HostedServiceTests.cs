using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests
{
    public class HostedServiceTests
    {
        [Test]
        public void AddHostedServiceTest()
        {
            GivenCollection();
            WhenAddImplementationService();
            ThenHostedServiceAdded();
        }

        [Test]
        public void AddServiceTest()
        {
            GivenCollection();
            WhenAddService();
            ThenServiceAndHostedServiceAreSame<IObjectUnderTest>();
        }

        [Test]
        public void AddInstanceTest()
        {
            GivenCollection();
            WhenAddInstance();
            ThenHostedServiceAdded();
        }

        [Test]
        public void AddInstanceAsServiceTest()
        {
            GivenCollection();
            WhenAddInstanceAsService();
            ThenHostedServiceAdded();
            ThenServiceAndHostedServiceAreSame<IObjectUnderTest>();
        }

        [Test]
        public void AddImplementationFactoryTest()
        {
            GivenCollection();
            WhenAddImplementationFactory();
            ThenHostedServiceAdded();
        }

        [Test]
        public void AddImplementationFactoryAsServiceTest()
        {
            GivenCollection();
            WhenAddImplementationFactoryAsService();
            ThenHostedServiceAdded();
            ThenServiceAndHostedServiceAreSame<IObjectUnderTest>();
        }

        [Test]
        public void HostedServiceAndServiceAreSameTest()
        {
            GivenCollection();
            WhenAddHostedService();
            ThenHostedServiceAdded();
            ThenServiceAndHostedServiceAreSame<IHostedServiceUnderTest>();
        }

        [Test]
        public void AddServiceTwiceTest()
        {
            GivenCollection();
            WhenAddService();
            WhenAddDifferentImplementation();
            ThenTwoServicesAdded();
        }

        [Test]
        public void AddSameServiceTwiceTest()
        {
            GivenCollection();
            WhenAddService();
            WhenAddService();
            ThenTwoServicesAdded();
        }

        [Test]
        public void NothingNotNeededAddedTest()
        {
            GivenCollection();
            WhenAddService();
            ThenOnlyNeededAdded();
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

        private void WhenAddImplementationService()
        {
            _serviceCollection.AddSingleton<HostedObjectUnderTest>();
        }

        private void WhenAddService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, HostedObjectUnderTest>();
        }

        private void WhenAddInstance()
        {
            _serviceCollection.AddSingleton(new HostedObjectUnderTest());
        }

        private void WhenAddInstanceAsService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest>(new HostedObjectUnderTest());
        }

        private void WhenAddImplementationFactory()
        {
            _serviceCollection.AddSingleton(serviceProvider => new HostedObjectUnderTest());
        }

        private void WhenAddImplementationFactoryAsService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, HostedObjectUnderTest>(serviceProvider => new HostedObjectUnderTest());
        }

        private void WhenAddHostedService()
        {
            _serviceCollection.AddSingleton<IHostedServiceUnderTest, HostedServiceUnderTest>();
        }

        private void WhenAddDifferentImplementation()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, HostedServiceUnderTest>();
        }

        private void ThenHostedServiceAdded()
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

        private void ThenTwoServicesAdded()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var hostedServices = serviceProvider.GetServices<IHostedService>().ToArray();
            var hostedServiceUnderTests = serviceProvider.GetServices<IObjectUnderTest>().ToArray();

            Assert.That(hostedServices.Count(), Is.EqualTo(2));
            Assert.That(hostedServiceUnderTests.Count(), Is.EqualTo(2));

            Assert.AreEqual(hostedServices[0], hostedServiceUnderTests[0]);
            Assert.AreEqual(hostedServices[1], hostedServiceUnderTests[1]);
        }

        private void ThenOnlyNeededAdded()
        {
            Assert.That(_serviceCollection.Count, Is.EqualTo(2));
            Assert.AreEqual(typeof(IObjectUnderTest), _serviceCollection[0].ServiceType);
            Assert.AreEqual(typeof(IHostedService), _serviceCollection[1].ServiceType);
        }
    }
}
