using System;
using Microsoft.Extensions.Hosting;
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
            ((CommonDecorator<INotDecoratedIntegrationTestClass>) _decorated).Enter += () => _hasEntered = true;
        }

        private void WhenCallNotDecoratedByAll()
        {
            //To check if the disposable decorator worked, we call the dispose method
            //If then the NotDecoratedByAll gets called and it would be decorated, an exception would be thrown 
            ((IDisposable)_decorated).Dispose();

            ((INotDecoratedIntegrationTestClass)_decorated).NotDecoratedByAll();
        }

        private void ThenNoDecoration()
        {
            Assert.That(_hasEntered, Is.False);
            Assert.That(_logger.Logged, Is.Empty);
        }
    }
}
