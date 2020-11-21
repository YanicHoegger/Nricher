using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DependencyInjectionExtensions.AddImplementation;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.AddImplementation
{
    public class AddImplementationServiceCollectionExtensionsTests
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

        private IServiceCollection _serviceCollection;

        private void GivenServiceCollectionWithExtension()
        {
            _serviceCollection = new ServiceCollectionExtender(new ServiceCollection(), new[] { new NotTransientImplementationExtension() });
        }

        private void GivenServiceCollectionWithoutExtension()
        {
            _serviceCollection = new ServiceCollection();
        }

        private void WhenAddService(Expression<Action<IServiceCollection>> addServiceAction)
        {
            addServiceAction.Compile()(_serviceCollection);
        }

        private void ThenImplementationAdded()
        {
            Assert.That(_serviceCollection.Count(x => x.ServiceType == typeof(IObjectUnderTest)), Is.EqualTo(1));
            Assert.That(_serviceCollection.Count(x => x.ServiceType == typeof(ObjectUnderTest)), Is.EqualTo(1));
        }
    }
}
