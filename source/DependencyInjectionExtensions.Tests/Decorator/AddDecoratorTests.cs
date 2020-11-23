using System.Collections.Generic;
using System.Linq;
using DependencyInjectionExtensions.Decorator;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class AddDecoratorTests
    {
        [Test]
        public void AddAsInstanceWithOneDecoratorTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddAsInstance();
            ThenOneDecorated();
        }

        [Test]
        public void AddImplementationFactoryWithOneDecoratorTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddImplementationFactory();
            ThenOneDecorated();
        }

        [Test]
        public void AddTypeWithOneDecoratorTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddType();
            ThenOneDecorated();
        }

        [Test]
        public void AddInstanceWithTwoDecoratorTest()
        {
            GivenCollectionWithTwoDecorator();
            WhenAddAsInstance();
            ThenTwoDecorated();
        }

        [Test]
        public void AddImplementationFactoryWithTwoDecoratorTest()
        {
            GivenCollectionWithTwoDecorator();
            WhenAddImplementationFactory();
            ThenTwoDecorated();
        }

        [Test]
        public void AddTypeWithTwoDecoratorTest()
        {
            GivenCollectionWithTwoDecorator();
            WhenAddType();
            ThenTwoDecorated();
        }

        [Test]
        public void SameDecoratorTwiceOnlyOnceDecoratedTest()
        {
            GivenCollectionWithOneDecoratorAddedTwice();
            WhenAddType();
            ThenOneDecoratedTwice();
        }

        [Test]
        public void AddServiceReturnsOneServicesTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddAsInstance();
            ThenReturnOneServices();
        }

        [Test]
        public void SameServiceAddedReturnsBothServicesTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddServiceTwice();
            ThenReturnsBothServices();
        }

        [Test]
        public void AddSameServiceTwiceReturnsLastAddedTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddServiceTwice();
            ThenReturnsLastAdded();
        }

        [Test]
        public void AddSingletonIsSingletonTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddType();
            ThenInstanceIsSingleton();
        }

        [Test]
        public void AddTransientAreTransientTest()
        {
            GivenCollectionWithOneDecorator();
            WhenAddTransient();
            ThenInstancesAreTransient();
        }

        private IServiceCollection _serviceCollection;

        private void GivenCollectionWithOneDecorator()
        {
            var decoratorOne = new DecoratorExtension(new DecoratorOneFactory());
            CreateServiceCollection(new[] { decoratorOne });
        }

        private void GivenCollectionWithTwoDecorator()
        {
            var decoratorOne = new DecoratorExtension(new DecoratorOneFactory());
            var decoratorTwo = new DecoratorExtension(new DecoratorTwoFactory());

            CreateServiceCollection(new[] { decoratorOne, decoratorTwo });
        }

        private void GivenCollectionWithOneDecoratorAddedTwice()
        {
            var firstDecorator = new DecoratorExtension(new DecoratorOneFactory());
            var secondDecorator = new DecoratorExtension(new DecoratorOneFactory());

            CreateServiceCollection(new[] { firstDecorator, secondDecorator });
        }

        private void WhenAddAsInstance()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest>(new ObjectUnderTest());
        }

        private void WhenAddImplementationFactory()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest>(serviceProvider => new ObjectUnderTest());
        }

        private void WhenAddType()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenAddServiceTwice()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>();
            _serviceCollection.AddSingleton<IObjectUnderTest, OtherObjectUnderTest>();
        }

        private void WhenAddTransient()
        {
            _serviceCollection.AddTransient<IObjectUnderTest, ObjectUnderTest>();
        }

        private void ThenOneDecorated()
        {
            var decorated = _serviceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            var decoratedInstance = GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(decorated);

            Assert.IsInstanceOf<ObjectUnderTest>(decoratedInstance);
        }

        private void ThenTwoDecorated()
        {
            var decorated = _serviceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            var decoratedUnderTwo = GetDecoratedOf<DecoratorTwo<IObjectUnderTest>, IObjectUnderTest>(decorated);
            var decoratedUnderOne = GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(decoratedUnderTwo);

            Assert.IsInstanceOf<ObjectUnderTest>(decoratedUnderOne);
        }

        private void ThenOneDecoratedTwice()
        {
            var decorated = _serviceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            var decoratedFirst = GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(decorated);
            var decoratedSecond = GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(decoratedFirst);

            Assert.IsInstanceOf<ObjectUnderTest>(decoratedSecond);
        }

        private void ThenReturnOneServices()
        {
            var services = _serviceCollection.BuildServiceProvider().GetServices<IObjectUnderTest>();
            Assert.That(services.Count(), Is.EqualTo(1));
        }

        private void ThenReturnsBothServices()
        {
            var services = _serviceCollection.BuildServiceProvider().GetServices<IObjectUnderTest>().ToArray();
            Assert.That(services.Length, Is.EqualTo(2));

            Assert.IsInstanceOf<ObjectUnderTest>(GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(services[0]));
            Assert.IsInstanceOf<OtherObjectUnderTest>(GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(services[1]));
        }

        private void ThenReturnsLastAdded()
        {
            var service = _serviceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();
            Assert.IsInstanceOf<OtherObjectUnderTest>(GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(service));
        }

        private void ThenInstanceIsSingleton()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var serviceOne = serviceProvider.GetService<IObjectUnderTest>();
            var serviceTwo = serviceProvider.GetService<IObjectUnderTest>();

            Assert.AreSame(serviceOne, serviceTwo);
        }

        private void ThenInstancesAreTransient()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            var serviceOne = serviceProvider.GetService<IObjectUnderTest>();
            var serviceTwo = serviceProvider.GetService<IObjectUnderTest>();

            Assert.AreNotSame(serviceOne, serviceTwo);
        }

        private static TDecorated GetDecoratedOf<TDecoratorBase, TDecorated>(object decorated)
            where TDecoratorBase : DecoratorBase<TDecorated>
        {
            Assert.IsTrue(decorated is TDecoratorBase);
            return ((TDecoratorBase) decorated).Decorated;
        }

        private void CreateServiceCollection(IEnumerable<IServiceCollectionExtension> extensions)
        {
            var decorated = new ServiceCollection();
            _serviceCollection = new ServiceCollectionExtender(decorated, extensions);
        }
    }
}
