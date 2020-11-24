using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DependencyInjectionExtensions.Decorator;
using DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class AddDecoratedServiceCollectionExtensionsTests : ExtensionMethodsTestsBase<SingleTypeDecoratorExtension<IObjectUnderTest, TestDecorator>>
    {
        [TestCaseSource(nameof(TestCaseData))]
        public void AddDecoratedTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithExtension();
            WhenAddService(addServiceAction);
            ThenDecorated();
        }

        [TestCaseSource(nameof(TestCaseData))]
        public void AddDecoratedOnNormalServiceCollectionTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithoutExtension();
            WhenAddService(addServiceAction);
            ThenDecorated();
        }

        private static readonly IDecoratorFactory DecoratorFactory = new SingleTypeDecoratorFactory<IObjectUnderTest, TestDecorator>();

        private static readonly IEnumerable<Expression<Action<IServiceCollection>>> TestCaseData = new Expression<Action<IServiceCollection>>[]
        {
            x => x.AddDecoratedSingleton<IObjectUnderTest, ObjectUnderTest>(DecoratorFactory),
            x => x.AddDecoratedSingleton<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest(), DecoratorFactory),
            x => x.AddDecoratedSingleton<IObjectUnderTest>(new ObjectUnderTest(), DecoratorFactory),
            x => x.AddDecoratedScoped<IObjectUnderTest, ObjectUnderTest>(DecoratorFactory),
            x => x.AddDecoratedScoped<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest(), DecoratorFactory),
            x => x.AddDecoratedTransient<IObjectUnderTest, ObjectUnderTest>(DecoratorFactory),
            x => x.AddDecoratedTransient<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest(), DecoratorFactory),
        };

        private void ThenDecorated()
        {
            var service = ServiceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            Assert.IsInstanceOf<TestDecorator>(service);
            var decorated = ((TestDecorator) service).Decorated;

            Assert.IsInstanceOf<ObjectUnderTest>(decorated);
        }
    }
}
