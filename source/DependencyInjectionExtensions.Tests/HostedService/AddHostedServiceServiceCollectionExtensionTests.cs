using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DependencyInjectionExtensions.HostedService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.HostedService
{
    public class AddHostedServiceServiceCollectionExtensionTests : ExtensionMethodsTestsBase<HostedServiceExtension>
    {
        [TestCaseSource(nameof(TestCaseData))]
        public void AddHostedServiceTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithExtension();
            WhenAddService(addServiceAction);
            ThenHostedServiceAdded();
        }

        [TestCaseSource(nameof(TestCaseData))]
        public void AddHostedServiceTestsOnNormalServiceCollectionTests(Expression<Action<IServiceCollection>> addServiceAction)
        {
            GivenServiceCollectionWithoutExtension();
            WhenAddService(addServiceAction);
            ThenHostedServiceAdded();
        }

        private static readonly IEnumerable<Expression<Action<IServiceCollection>>> TestCaseData = new Expression<Action<IServiceCollection>>[]
        {
            x => x.AddSingletonHostedService<IObjectUnderTest, HostedObjectUnderTest>(),
            x => x.AddSingletonHostedService<IObjectUnderTest, HostedObjectUnderTest>(y => new HostedObjectUnderTest()),
            x => x.AddSingletonHostedService<IObjectUnderTest, HostedObjectUnderTest>(new HostedObjectUnderTest()),
        };

        private void ThenHostedServiceAdded()
        {
            Assert.That(ServiceCollection.Count(x => x.ServiceType == typeof(IObjectUnderTest)), Is.EqualTo(1));
            Assert.That(ServiceCollection.Count(x => x.ServiceType == typeof(IHostedService)), Is.EqualTo(1));
        }
    }
}
