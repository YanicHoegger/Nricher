using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests
{
    public class RegisterServiceAndImplementationTests
    {
        [Test]
        public void RegisterSingletonTest()
        {
            GivenCollection();
            WhenRegisterSingleton();
            ThenImplementationAndInterfaceAreSame();
        }

        [Test]
        public void RegisterAsTransientTest()
        {
            GivenCollection();
            WhenRegisterAsTransient();
            ThenImplementationNotRegistered();
        }

        [Test]
        public void RegisterScopedAreSameInScope()
        {
            GivenCollection();
            WhenRegisterScoped();
            ThenImplementationAndInterfaceAreSameWithinScope();
        }

        [Test]
        public void RegisterScopedAreDifferentOnDifferentScopes()
        {
            GivenCollection();
            WhenRegisterScoped();
            ThenDifferentObjectsInScopes();
        }

        [Test]
        public void RegisterInstance()
        {
            GivenCollection();
            WhenRegisterInstance();
            ThenImplementationAndInterfaceAreSame();
            ThenRegisteredIsSameAsInstance();
        }

        [Test]
        public void RegisterSingletonImplementationFactory()
        {
            GivenCollection();
            WhenRegisterSingletonImplementationFactory();
            ThenImplementationAndInterfaceAreSame();
        }

        [Test]
        public void RegisterScopedImplementationFactory()
        {
            GivenCollection();
            WhenRegisterScopedImplementationFactory();
            ThenImplementationAndInterfaceAreSameWithinScope();
            ThenDifferentObjectsInScopes();
        }

        private IServiceCollection _serviceCollection;
        private ObjectUnderTest _registeredInstance;

        private void GivenCollection()
        {
            var decorated = new ServiceCollection();
            var singletonExtender = new NotTransientImplementationExtension();

            _serviceCollection = new ServiceCollectionExtender(decorated, new[] { singletonExtender });
        }

        private void WhenRegisterSingleton()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenRegisterAsTransient()
        {
            _serviceCollection.AddTransient<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenRegisterScoped()
        {
            _serviceCollection.AddScoped<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenRegisterInstance()
        {
            _registeredInstance = new ObjectUnderTest();
            _serviceCollection.AddSingleton<IObjectUnderTest>(_registeredInstance);
        }

        private void WhenRegisterSingletonImplementationFactory()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>(serviceProvider => new ObjectUnderTest());
        }

        private void WhenRegisterScopedImplementationFactory()
        {
            _serviceCollection.AddScoped<IObjectUnderTest, ObjectUnderTest>(serviceProvider => new ObjectUnderTest());
        }

        private void ThenImplementationAndInterfaceAreSame()
        {
            var serviceProvider = GetServiceProvider();

            Assert.AreSame(serviceProvider.GetService<IObjectUnderTest>(), serviceProvider.GetService<ObjectUnderTest>());
        }

        private void ThenImplementationNotRegistered()
        {
            Assert.IsNull(GetServiceProvider().GetService<ObjectUnderTest>());
        }

        private void ThenImplementationAndInterfaceAreSameWithinScope()
        {
            var scoped = GetServiceProvider().CreateScope().ServiceProvider;

            Assert.AreSame(scoped.GetService<IObjectUnderTest>(), scoped.GetService<ObjectUnderTest>());
        }

        private void ThenDifferentObjectsInScopes()
        {
            var scopeOne = GetServiceProvider().CreateScope().ServiceProvider;
            var scopeTwo = GetServiceProvider().CreateScope().ServiceProvider;

            Assert.AreNotSame(scopeOne.GetService<IObjectUnderTest>(), scopeTwo.GetService<IObjectUnderTest>());
            Assert.AreNotSame(scopeOne.GetService<ObjectUnderTest>(), scopeTwo.GetService<ObjectUnderTest>());
            Assert.AreNotSame(scopeOne.GetService<ObjectUnderTest>(), scopeTwo.GetService<IObjectUnderTest>());
        }

        private void ThenRegisteredIsSameAsInstance()
        {
            Assert.AreSame(_registeredInstance, GetServiceProvider().GetService<ObjectUnderTest>());
            Assert.AreSame(_registeredInstance, GetServiceProvider().GetService<IObjectUnderTest>());
        }

        private ServiceProvider GetServiceProvider()
        {
            return _serviceCollection.BuildServiceProvider();
        }
    }
}
