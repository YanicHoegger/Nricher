using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public class RegisterDecoratorTests
    {
        [Test]
        public void RegisterAsInstanceWithOneDecoratorTest()
        {
            GivenCollectionWithOneDecorator();
            WhenRegisterAsInstance();
            ThenOneDecorated();
        }

        [Test]
        public void RegisterImplementationFactoryWithOneDecoratorTest()
        {
            GivenCollectionWithOneDecorator();
            WhenRegisterImplementationFactory();
            ThenOneDecorated();
        }

        [Test]
        public void RegisterTypeWithOneDecoratorTest()
        {
            GivenCollectionWithOneDecorator();
            WhenRegisterType();
            ThenOneDecorated();
        }

        [Test]
        public void RegisterInstanceWithTwoDecoratorTest()
        {
            GivenCollectionWithTwoDecorator();
            WhenRegisterAsInstance();
            ThenTwoDecorated();
        }

        [Test]
        public void RegisterImplementationFactoryWithTwoDecoratorTest()
        {
            GivenCollectionWithTwoDecorator();
            WhenRegisterImplementationFactory();
            ThenTwoDecorated();
        }

        [Test]
        public void RegisterTypeWithTwoDecoratorTest()
        {
            GivenCollectionWithTwoDecorator();
            WhenRegisterType();
            ThenTwoDecorated();
        }

        [Test]
        public void SameDecoratorTwiceOnlyOnceDecoratedTest()
        {
            GivenCollectionWithOneDecoratorRegisteredTwice();
            WhenRegisterType();
            ThenOneDecoratedTwice();
        }

        [Test]
        public void RegisterServiceReturnsOneServicesTest()
        {
            GivenCollectionWithOneDecorator();
            WhenRegisterAsInstance();
            ThenReturnOneServices();
        }

        [Test]
        public void SameServiceRegisteredReturnsBothServicesTest()
        {
            GivenCollectionWithOneDecorator();
            WhenRegisterServiceTwice();
            ThenReturnsBothServices();
        }

        [Test]
        public void RegisterSameServiceTwiceReturnsLastRegisteredTest()
        {
            GivenCollectionWithOneDecorator();
            WhenRegisterServiceTwice();
            ThenReturnsLastRegistered();
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

        private void GivenCollectionWithOneDecoratorRegisteredTwice()
        {
            var firstDecorator = new DecoratorExtension(new DecoratorOneFactory());
            var secondDecorator = new DecoratorExtension(new DecoratorOneFactory());

            CreateServiceCollection(new[] { firstDecorator, secondDecorator });
        }

        private void WhenRegisterAsInstance()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest>(new ObjectUnderTest());
        }

        private void WhenRegisterImplementationFactory()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest>(serviceProvider => new ObjectUnderTest());
        }

        private void WhenRegisterType()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenRegisterServiceTwice()
        {
            _serviceCollection.AddSingleton<IObjectUnderTest, ObjectUnderTest>();
            _serviceCollection.AddSingleton<IObjectUnderTest, OtherObjectUnderTest>();
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

        private void CreateServiceCollection(IEnumerable<IServiceCollectionExtension> extensions)
        {
            var decorated = new ServiceCollection();
            _serviceCollection = ServiceCollectionExtender.Create(decorated, extensions);
        }

        private static TDecorated GetDecoratedOf<TDecoratorBase, TDecorated>(object decorated)
            where TDecoratorBase : DecoratorBase<TDecorated>
        {
            Assert.IsTrue(decorated is TDecoratorBase);
            return ((TDecoratorBase) decorated).Decorated;
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

        private void ThenReturnsLastRegistered()
        {
            var service = _serviceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();
            Assert.IsInstanceOf<OtherObjectUnderTest>(GetDecoratedOf<DecoratorOne<IObjectUnderTest>, IObjectUnderTest>(service));
        }
    }
}
