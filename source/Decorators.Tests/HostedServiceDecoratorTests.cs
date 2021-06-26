using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Nricher.Decorators;
using NUnit.Framework;

namespace Decorators.Tests
{
    public class HostedServiceDecoratorTests
    {
        [Test]
        public void WhenNotStartedThenThrowsTest()
        {
            GivenDecorated();
            WhenNotStarted();
            ThenThrowsWhenCallingMethod();
        }

        [Test]
        public void WhenStartedThenDoesNotThrowTest()
        {
            GivenDecorated();
            WhenStarted();
            ThenDoesNotThrowWhenCallingMethod();
        }

        [Test]
        public void WhenStartedAndStoppedThenThrowsTest()
        {
            GivenDecorated();
            WhenStarted();
            WhenStopped();
            ThenThrowsWhenCallingMethod();
        }

        [Test]
        public void WhenNotStartedThenNotThrowsWhenNotDecoratedTest()
        {
            GivenDecorated();
            WhenNotStarted();
            ThenDoesNotThrowWhenCallingNotDecoratedMethod();
        }

        private object _decorated;

        private void GivenDecorated()
        {
            _decorated = HostedServiceDecorator.Create(new HostedService());
        }

        private static void WhenNotStarted()
        {
            //Nothing to do here
        }

        private void WhenStarted()
        {
            ((IHostedService)_decorated).StartAsync(CancellationToken.None).Wait();
        }

        private void WhenStopped()
        {
            ((IHostedService)_decorated).StopAsync(CancellationToken.None).Wait();
        }

        private void ThenThrowsWhenCallingMethod()
        {
            Assert.Throws<InvalidOperationException>(CallingMethod);
        }

        private void ThenDoesNotThrowWhenCallingMethod()
        {
            CallingMethod();
        }

        private void ThenDoesNotThrowWhenCallingNotDecoratedMethod()
        {
            ((ISomeAction)_decorated).NotDecoratedAction();
        }

        private void CallingMethod()
        {
            ((ISomeAction)_decorated).SomeAction();
        }
    }
}
