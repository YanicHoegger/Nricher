using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nricher.DependencyInjectionExtensions;
using Nricher.DependencyInjectionExtensions.AddImplementation;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.AddImplementation
{
    public class AddServiceAndImplementationTests
    {
        [Test]
        public void AddSingletonTest()
        {
            GivenCollection();
            WhenAddSingleton();
            ThenImplementationAndInterfaceAreSame();
        }

        [Test]
        public void AddAsTransientTest()
        {
            GivenCollection();
            WhenAddAsTransient();
            ThenImplementationNotAdded();
        }

        [Test]
        public void AddScopedAreSameInScope()
        {
            GivenCollection();
            WhenAddScoped();
            ThenImplementationAndInterfaceAreSameWithinScope();
        }

        [Test]
        public void AddScopedAreDifferentOnDifferentScopes()
        {
            GivenCollection();
            WhenAddScoped();
            ThenDifferentObjectsInScopes();
        }

        [Test]
        public void AddInstance()
        {
            GivenCollection();
            WhenAddInstance();
            ThenImplementationAndInterfaceAreSame();
            ThenAddedIsSameAsInstance();
        }

        [Test]
        public void AddSingletonImplementationFactory()
        {
            GivenCollection();
            WhenAddSingletonImplementationFactory();
            ThenImplementationAndInterfaceAreSame();
        }

        [Test]
        public void AddScopedImplementationFactory()
        {
            GivenCollection();
            WhenAddScopedImplementationFactory();
            ThenImplementationAndInterfaceAreSameWithinScope();
            ThenDifferentObjectsInScopes();
        }

        [Test]
        public void AddSingletonReturnsOnlyOneServices()
        {
            GivenCollection();
            WhenAddSingleton();
            ThenOnlyOneServices();
        }

        [Test]
        public void AddImplementationFactoryReturnsOnlyOneServices()
        {
            GivenCollection();
            WhenAddSingletonImplementationFactory();
            ThenOnlyOneServices();
        }

        [Test]
        public void AddTwoDifferentServices()
        {
            GivenCollection();
            WhenAddSingleton();
            WhenAddAnOtherSingleton();
            ThenTwoServices();
        }

        [Test]
        public void AddSingletonImplementationFactoryAsService()
        {
            GivenCollection();
            WhenAddSingletonImplementationFactoryAsService();
            ThenNoImplementationAdded();
        }

        private IServiceCollection _serviceCollection;
        private ObjectUnderTest _addedInstance;

        private void GivenCollection()
        {
            var decorated = new ServiceCollection();
            var singletonExtender = new NotTransientImplementationExtension();

            _serviceCollection = new ServiceCollectionExtender(decorated, new[] { singletonExtender });
        }

        private void WhenAddSingleton()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenAddAnOtherSingleton()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, OtherObjectUnderTest>();
        }

        private void WhenAddAsTransient()
        {
            _serviceCollection.AddTransient<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenAddScoped()
        {
            _serviceCollection.AddScoped<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenAddInstance()
        {
            _addedInstance = new ObjectUnderTest();
            _serviceCollection.AddSingleton<IObjectUnderTest>(_addedInstance);
        }

        private void WhenAddSingletonImplementationFactory()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>(_ => new ObjectUnderTest());
        }

        private void WhenAddScopedImplementationFactory()
        {
            _serviceCollection.AddScoped<IObjectUnderTest, ObjectUnderTest>(_ => new ObjectUnderTest());
        }

        private void WhenAddSingletonImplementationFactoryAsService()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest>(_ => new ObjectUnderTest());
        }

        private void ThenImplementationAndInterfaceAreSame()
        {
            var serviceProvider = GetServiceProvider();

            Assert.AreSame(serviceProvider.GetService<IObjectUnderTest>(), serviceProvider.GetService<ObjectUnderTest>());
        }

        private void ThenImplementationNotAdded()
        {
            Assert.IsNull(GetServiceProvider().GetService<ObjectUnderTest>());
        }

        private void ThenImplementationAndInterfaceAreSameWithinScope()
        {
            using var serviceScope = GetServiceProvider().CreateScope();
            var scoped = serviceScope.ServiceProvider;

            Assert.AreSame(scoped.GetService<IObjectUnderTest>(), scoped.GetService<ObjectUnderTest>());
        }

        private void ThenDifferentObjectsInScopes()
        {
            using var scopeOne = GetServiceProvider().CreateScope();
            using var scopeTwo = GetServiceProvider().CreateScope();

            var providerOne = scopeOne.ServiceProvider;
            var providerTwo = scopeTwo.ServiceProvider;

            Assert.AreNotSame(providerOne.GetService<IObjectUnderTest>(), providerTwo.GetService<IObjectUnderTest>());
            Assert.AreNotSame(providerOne.GetService<ObjectUnderTest>(), providerTwo.GetService<ObjectUnderTest>());
            Assert.AreNotSame(providerOne.GetService<ObjectUnderTest>(), providerTwo.GetService<IObjectUnderTest>());
        }

        private void ThenAddedIsSameAsInstance()
        {
            Assert.AreSame(_addedInstance, GetServiceProvider().GetService<ObjectUnderTest>());
            Assert.AreSame(_addedInstance, GetServiceProvider().GetService<IObjectUnderTest>());
        }

        private void ThenOnlyOneServices()
        {
            Assert.That(GetServiceProvider().GetServices<IObjectUnderTest>().Count(), Is.EqualTo(1));
            Assert.That(GetServiceProvider().GetServices<ObjectUnderTest>().Count(), Is.EqualTo(1));
        }

        private void ThenTwoServices()
        {
            Assert.That(GetServiceProvider().GetServices<IObjectUnderTest>().Count(), Is.EqualTo(2));
            Assert.That(GetServiceProvider().GetServices<ObjectUnderTest>().Count(), Is.EqualTo(1));
            Assert.That(GetServiceProvider().GetServices<OtherObjectUnderTest>().Count(), Is.EqualTo(1));
        }

        private void ThenNoImplementationAdded()
        {
            Assert.IsNull(GetServiceProvider().GetService<ObjectUnderTest>());
            Assert.IsNotNull(GetServiceProvider().GetService<IObjectUnderTest>());
        }

        private ServiceProvider GetServiceProvider()
        {
            return _serviceCollection.BuildServiceProvider();
        }
    }
}
