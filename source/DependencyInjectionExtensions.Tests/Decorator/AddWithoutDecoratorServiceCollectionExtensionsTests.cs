using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DependencyInjectionExtensions.Decorator;
using DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class AddWithoutDecoratorServiceCollectionExtensionsTests : ExtensionMethodsTestsBase<SingleTypeDecoratorExtension<IObjectUnderTest, TestDecorator>>
    {
        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddImplementationTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithExtension();
            WhenAddService(addServiceAction);
            ThenNotDecorated();
        }

        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddImplementationOnNormalServiceCollectionTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithoutExtension();
            WhenAddService(addServiceAction);
            ThenNotDecorated();
        }

        private static readonly IEnumerable<Expression<Action<IServiceCollection>>> TestCaseData = new Expression<Action<IServiceCollection>>[]
        {
            x => x.AddSingletonWithoutDecorator<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddSingletonWithoutDecorator<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest()),
            x => x.AddSingletonWithoutDecorator<IObjectUnderTest>(new ObjectUnderTest()),
            x => x.AddScopedWithoutDecorator<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddScopedWithoutDecorator<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest()),
            x => x.AddTransientWithoutDecorator<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddTransientWithoutDecorator<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest())
        };

        private void ThenNotDecorated()
        {
            var service = ServiceCollection.BuildServiceProvider().GetService<IObjectUnderTest>();

            Assert.IsInstanceOf<ObjectUnderTest>(service);
        }
    }
}
