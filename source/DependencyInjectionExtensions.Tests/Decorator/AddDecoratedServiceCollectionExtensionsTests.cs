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
            x => x.AddDecoratedSingleton<IObjectUnderTest, ObjectUnderTest, TestDecorator>(),
            x => x.AddDecoratedSingleton<IObjectUnderTest, ObjectUnderTest, TestDecorator>(y => new ObjectUnderTest()),
            x => x.AddDecoratedSingleton<IObjectUnderTest, TestDecorator>(new ObjectUnderTest()),
            x => x.AddDecoratedScoped<IObjectUnderTest, ObjectUnderTest>(DecoratorFactory),
            x => x.AddDecoratedScoped<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest(), DecoratorFactory),
            x => x.AddDecoratedScoped<IObjectUnderTest, ObjectUnderTest, TestDecorator>(),
            x => x.AddDecoratedScoped<IObjectUnderTest, ObjectUnderTest, TestDecorator>(y => new ObjectUnderTest()),
            x => x.AddDecoratedTransient<IObjectUnderTest, ObjectUnderTest>(DecoratorFactory),
            x => x.AddDecoratedTransient<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest(), DecoratorFactory),
            x => x.AddDecoratedTransient<IObjectUnderTest, ObjectUnderTest, TestDecorator>(),
            x => x.AddDecoratedTransient<IObjectUnderTest, ObjectUnderTest, TestDecorator>(y => new ObjectUnderTest())
        };

        private void ThenDecorated()
        {
            var service = ServiceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            Assert.IsInstanceOf<TestDecorator>(service);
            // ReSharper disable once PossibleNullReferenceException : Is ok for a test
            var decorated = ((TestDecorator) service).Decorated;

            Assert.IsInstanceOf<ObjectUnderTest>(decorated);
        }
    }
}
