using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Nricher.Decorators;
using NUnit.Framework;

namespace Decorators.Tests
{
    public class NotDecoratedIntegrationTests
    {
        [Test]
        public void NoDecorationByAllDecoratorsTest()
        {
            GivenDecorated();
            WhenCallNotDecoratedByAll();
            ThenNoDecoration();
        }

        [Test]
        public void NoDecorationByCommonDecoratorTest()
        {
            GivenDecorated();
            WhenCallNotDecoratedByCommonDecorator();
            ThenNoCommonDecoratorDecoration();
        }

        [Test]
        public void NoDecorationByLoggingDecoratorTest()
        {
            GivenDecorated();
            WhenCallNotDecoratedByLoggingDecorator();
            ThenNoLoggingDecoratorDecoration();
        }

        [Test]
        public void NoDecorationByDisposableDecoratorTest()
        {
            GivenDecorated();
            WhenCallNotDecoratedByDisposableDecorator();
            ThenNoDisposableDecoratorDecoration();
        }

        [Test]
        public void NoDecorationByHostedServiceDecorator()
        {
            GivenDecorated();
            WhenCallNotDecoratedByHostedServiceDecorator();
            ThenNoHostedServiceDecoratorDecoration();
        }

        private object _decorated;
        private LoggerMock<INotDecoratedIntegrationTestClass> _logger;
        private bool _hasEntered;

        private void GivenDecorated()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var toDecorate = new NotDecoratedIntegrationTestClass();
#pragma warning restore CA2000 // Dispose objects before losing scope

            var disposableDecorated = DisposableDecorator.Create(toDecorate);
            var hostedDecorated = HostedServiceDecorator.Create((IHostedService)disposableDecorated);

            _logger = new LoggerMock<INotDecoratedIntegrationTestClass>();
            var loggerDecorated = LoggingDecorator<INotDecoratedIntegrationTestClass>.Create((INotDecoratedIntegrationTestClass)hostedDecorated, _logger);

            _decorated = CommonDecorator<INotDecoratedIntegrationTestClass>.Create(loggerDecorated);
            ((CommonDecorator<INotDecoratedIntegrationTestClass>)_decorated).Enter += () => _hasEntered = true;
        }

        private void WhenCallNotDecoratedByAll()
        {
            DisposeAndPreparePrecondition();

            ((INotDecoratedIntegrationTestClass)_decorated).NotDecoratedByAll();
        }


        private void WhenCallNotDecoratedByCommonDecorator()
        {
            StartAndPreparePrecondition();

            ((INotDecoratedIntegrationTestClass)_decorated).NotDecoratedByCommonDecorator();
        }

        private void WhenCallNotDecoratedByLoggingDecorator()
        {
            StartAndPreparePrecondition();

            ((INotDecoratedIntegrationTestClass)_decorated).NotDecoratedByLoggingDecorator();
        }

        private void WhenCallNotDecoratedByDisposableDecorator()
        {
            StartAndPreparePrecondition();
            DisposeAndPreparePrecondition();

            ((INotDecoratedIntegrationTestClass)_decorated).NotDecoratedByDisposableDecorator();
        }

        private void WhenCallNotDecoratedByHostedServiceDecorator()
        {
            ((INotDecoratedIntegrationTestClass)_decorated).NotDecoratedByHostedServiceDecorator();
        }

        private void ThenNoDecoration()
        {
            Assert.That(_hasEntered, Is.False);
            Assert.That(_logger.Logged, Is.Empty);
        }

        private void ThenNoCommonDecoratorDecoration()
        {
            Assert.That(_hasEntered, Is.False);
            Assert.That(_logger.Logged, Is.Not.Empty);
        }

        private void ThenNoLoggingDecoratorDecoration()
        {
            Assert.That(_hasEntered, Is.True);
            Assert.That(_logger.Logged, Is.Empty);
        }

        private void ThenNoDisposableDecoratorDecoration()
        {
            Assert.That(_hasEntered, Is.True);
            Assert.That(_logger.Logged, Is.Not.Empty);
        }

        private void ThenNoHostedServiceDecoratorDecoration()
        {
            Assert.That(_hasEntered, Is.True);
            Assert.That(_logger.Logged, Is.Not.Empty);
        }

        private void StartAndPreparePrecondition()
        {
            //To check if still decorated by HostedServiceDecorator, if not decorated, an exception would be thrown
            ((IHostedService)_decorated).StartAsync(CancellationToken.None).Wait();

            PreparePrecondition();
        }

        private void DisposeAndPreparePrecondition()
        {
            //To check if the disposable decorator worked, we call the dispose method
            //If then the NotDecoratedByAll gets called and it would be decorated, an exception would be thrown 
            ((IDisposable)_decorated).Dispose();

            PreparePrecondition();
        }

        private void PreparePrecondition()
        {
            _hasEntered = false;
            _logger.Logged.Clear();
        }
    }
}
