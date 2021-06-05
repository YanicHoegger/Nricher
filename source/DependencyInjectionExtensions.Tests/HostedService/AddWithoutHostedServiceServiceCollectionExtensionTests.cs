using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Nricher.DependencyInjectionExtensions.HostedService;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.HostedService
{
    public class AddWithoutHostedServiceServiceCollectionExtensionTests : ExtensionMethodsTestsBase<HostedServiceExtension>
    {
        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddHostedServiceTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithExtension();
            WhenAddService(addServiceAction);
            ThenHostedServiceNotAdded();
        }

        [TestCaseSource(nameof(TestCaseData))]
        public void NotAddHostedServiceTestsOnNormalServiceCollectionTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithoutExtension();
            WhenAddService(addServiceAction);
            ThenHostedServiceNotAdded();
        }

        private static readonly IEnumerable<Expression<Action<IServiceCollection>>> TestCaseData = new Expression<Action<IServiceCollection>>[]
        {
            x => x.AddSingletonWithoutHostedService<IObjectUnderTest, HostedObjectUnderTest>(),
            x => x.AddSingletonWithoutHostedService<IObjectUnderTest, HostedObjectUnderTest>(y => new HostedObjectUnderTest()),
            x => x.AddSingletonWithoutHostedService<IObjectUnderTest, HostedObjectUnderTest>(new HostedObjectUnderTest())
        };

        private void ThenHostedServiceNotAdded()
        {
            CollectionAssert.IsEmpty(ServiceCollection.Where(x => x.ServiceType != typeof(IObjectUnderTest)));
            Assert.That(ServiceCollection.Count(x => x.ServiceType == typeof(IObjectUnderTest)), Is.EqualTo(1));
        }
    }
}
