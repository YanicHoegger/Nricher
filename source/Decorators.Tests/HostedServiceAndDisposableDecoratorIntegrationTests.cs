using System;
using NUnit.Framework;

namespace Decorators.Tests
{
    public class HostedServiceAndDisposableDecoratorIntegrationTests
    {
        [Test]
        public void CanDisposeWhenServiceStoppedTest()
        {
            GivenDecorators();
            WhenCallDispose();
            NoExceptionFromHostedServiceDecorator();
        }

        private object _decorated;

        private void GivenDecorators()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var toDecorate = new HostedServiceAndDisposableDecoratorIntegrationTestClass();
#pragma warning restore CA2000 // Dispose objects before losing scope

            var hostedServiceDecorated = HostedServiceDecorator.Create(toDecorate);
            _decorated = DisposableDecorator.Create((IDisposable) hostedServiceDecorated);
        }

        private void WhenCallDispose()
        {
            ((IDisposable)_decorated).Dispose();
        }

        private static void NoExceptionFromHostedServiceDecorator()
        {
            //Nothing to do here
        }
    }
}
