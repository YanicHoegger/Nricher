using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Nricher.DependencyInjectionExtensions.AddImplementation;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.AddImplementation
{
    public class AddImplementationServiceCollectionExtensionsTests : ExtensionMethodsTestsBase<NotTransientImplementationExtension>
    {
        [TestCaseSource(nameof(TestCaseData))]
        public void AddImplementationTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithExtension();
            WhenAddService(addServiceAction);
            ThenImplementationAdded();
        }

        [TestCaseSource(nameof(TestCaseData))]
        public void AddImplementationOnNormalServiceCollectionTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithoutExtension();
            WhenAddService(addServiceAction);
            ThenImplementationAdded();
        }

        private static readonly IEnumerable<Expression<Action<IServiceCollection>>> TestCaseData = new Expression<Action<IServiceCollection>>[]
        {
            x => x.AddSingletonWithImplementation<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddSingletonWithImplementation<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest()),
            x => x.AddSingletonWithImplementation<IObjectUnderTest>(new ObjectUnderTest()),
            x => x.AddScopedWithImplementation<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddScopedWithImplementation<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest())
        };

        private void ThenImplementationAdded()
        {
            Assert.That(ServiceCollection.Count(x => x.ServiceType == typeof(IObjectUnderTest)), Is.EqualTo(1));
            Assert.That(ServiceCollection.Count(x => x.ServiceType == typeof(ObjectUnderTest)), Is.EqualTo(1));
        }
    }
}
