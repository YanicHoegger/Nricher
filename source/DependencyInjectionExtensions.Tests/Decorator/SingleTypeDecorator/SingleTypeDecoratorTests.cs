using System;
using DependencyInjectionExtensions.Decorator;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator
{
    public class SingleTypeDecoratorTests
    {
        [Test]
        public void SingleTypeDecoratedTest()
        {
            GivenServiceCollectionWithDecorator<TestDecorator>();
            WhenAddService();
            ThenServiceDecorated<TestDecorator>();
        }

        [Test]
        public void MultipleParameterDecoratorTest()
        {
            GivenServiceCollectionWithDecorator<MultipleParameterDecorator>();
            WhenAddExtraParameter();
            WhenAddService();
            ThenServiceDecorated<MultipleParameterDecorator>();
        }

        [Test]
        public void ParameterNotAddedThrowsTest()
        {
            GivenServiceCollectionWithDecorator<MultipleParameterDecorator>();
            WhenAddService();
            ThenThrowsWhenGetService();
        }

        [Test]
        public void ValidMultipleConstructorTest()
        {
            GivenServiceCollectionWithDecorator<ValidMultipleConstructorDecorator>();
            WhenAddService();
            ThenServiceDecorated<ValidMultipleConstructorDecorator>();
        }

        [Test]
        public void InvalidMultipleConstructorTest()
        {
            GivenServiceCollectionWithDecoratorThrows<InvalidMultipleConstructorDecorator>();
        }

        [Test]
        public void MultipleDecoratorsTest()
        {
            GivenServiceCollectionWithDecorators();
            WhenAddService();
            WhenAddExtraParameter();
            ThenServiceMultipleDecorated();
        }

        private IServiceCollection _serviceCollection;

        private void GivenServiceCollectionWithDecorator<T>()
            where T : IObjectUnderTest
        {
            _serviceCollection = new ServiceCollectionExtender(new ServiceCollection(),
                new[] { new SingleTypeDecoratorExtension<IObjectUnderTest, T>() });
        }

        private void GivenServiceCollectionWithDecoratorThrows<T>()
            where T : IObjectUnderTest
        {
            Assert.Throws<InvalidOperationException>(GivenServiceCollectionWithDecorator<T>);
        }

        private void GivenServiceCollectionWithDecorators()
        {
            _serviceCollection = new ServiceCollectionExtender(new ServiceCollection(),
                new DecoratorExtension[]
                {
                    new SingleTypeDecoratorExtension<IObjectUnderTest, ValidMultipleConstructorDecorator>(),
                    new SingleTypeDecoratorExtension<IObjectUnderTest, MultipleParameterDecorator>(),
                    new SingleTypeDecoratorExtension<IObjectUnderTest, TestDecorator>()
                });
        }

        private void WhenAddService()
        {
            _serviceCollection.AddTransient<IObjectUnderTest, ObjectUnderTest>();
        }

        private void WhenAddExtraParameter()
        {
            _serviceCollection.AddTransient<ExtraParameter>();
        }

        private void ThenServiceDecorated<T>()
            where T : DecoratorBase
        {
            var service = _serviceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            var decorated = GetDecoratedOf<T>(service);

            Assert.IsInstanceOf<ObjectUnderTest>(decorated);
        }

        private void ThenThrowsWhenGetService()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService<IObjectUnderTest>());
        }

        private void ThenServiceMultipleDecorated()
        {
            var service = _serviceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            var decorated = GetDecoratedOf<TestDecorator>(service);
            decorated = GetDecoratedOf<MultipleParameterDecorator>(decorated);
            decorated = GetDecoratedOf<ValidMultipleConstructorDecorator>(decorated);

            Assert.IsInstanceOf<ObjectUnderTest>(decorated);
        }

        private static IObjectUnderTest GetDecoratedOf<T>(IObjectUnderTest objectUnderTest)
            where T : DecoratorBase
        {
            Assert.IsInstanceOf<T>(objectUnderTest);

            return ((T)objectUnderTest).Decorated;
        }
    }
}
