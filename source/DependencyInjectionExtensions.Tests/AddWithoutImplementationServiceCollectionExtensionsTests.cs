using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests
{
    public class AddWithoutImplementationServiceCollectionExtensionsTests
    {
        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddImplementationTests(Action<IServiceCollection> addServiceAction)
        {
            GivenServiceCollectionWithExtension();
            WhenAddService(addServiceAction);
            ThenImplementationNotAdded();
        }

        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddImplementationOnNormalServiceCollectionTests(Action<IServiceCollection> addServiceAction)
        {
            GivenServiceCollectionWithoutExtension();
            WhenAddService(addServiceAction);
            ThenImplementationNotAdded();
        }

        private static readonly IEnumerable<Action<IServiceCollection>> TestCaseData = new Action<IServiceCollection>[]
        {
            x => x.AddSingletonWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddSingletonWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest()),
            x => x.AddSingletonWithoutImplementation<IObjectUnderTest>(new ObjectUnderTest()),
            x => x.AddScopedWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(),
            x => x.AddScopedWithoutImplementation<IObjectUnderTest, ObjectUnderTest>(y => new ObjectUnderTest())
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

        private void WhenAddService(Action<IServiceCollection> addServiceAction)
        {
            addServiceAction(_serviceCollection);
        }

        private void ThenImplementationNotAdded()
        {
            CollectionAssert.IsEmpty(_serviceCollection.Where(x => x.ServiceType != typeof(IObjectUnderTest)));
            Assert.That(_serviceCollection.Count(x => x.ServiceType == typeof(IObjectUnderTest)), Is.EqualTo(1));
        }
    }
}
