using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DependencyInjectionExtensions.AddImplementation;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.AddImplementation
{
    public class AddWithoutImplementationServiceCollectionExtensionsTests : ExtensionMethodsTestsBase<NotTransientImplementationExtension>
    {
        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddImplementationTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithExtension();
            WhenAddService(addServiceAction);
            ThenImplementationNotAdded();
        }

        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddImplementationOnNormalServiceCollectionTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithoutExtension();
            WhenAddService(addServiceAction);
            ThenImplementationNotAdded();
        }

        private static readonly IEnumerable<Expression<Action<IServiceCollection>>> TestCaseData = new Expression<Action<IServiceCollection>>[]
        {
            x => x.AddSingletonWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddSingletonWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest()),
            x => x.AddSingletonWithoutImplementation<IObjectUnderTest>(new ObjectUnderTest()),
            x => x.AddScopedWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddScopedWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest())
        };

        private void ThenImplementationNotAdded()
        {
            CollectionAssert.IsEmpty(ServiceCollection.Where(x => x.ServiceType != typeof(IObjectUnderTest)));
            Assert.That(ServiceCollection.Count(x => x.ServiceType == typeof(IObjectUnderTest)), Is.EqualTo(1));
        }
    }
}
