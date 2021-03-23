using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Decorators.Tests
{
    public interface INotDecoratedIntegrationTestClass
    {
        void NotDecoratedByAll();
        void NotDecoratedByCommonDecorator();
        void NotDecoratedByDisposableDecorator();
        void NotDecoratedByHostedServiceDecorator();
        void NotDecoratedByLoggingDecorator();
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class NotDecoratedIntegrationTestClass : INotDecoratedIntegrationTestClass, IHostedService, IDisposable
    {
        [NotDecorated]
        public void NotDecoratedByAll()
        {
        }

        [NotDecorated(typeof(CommonDecorator<>))]
        public void NotDecoratedByCommonDecorator()
        {
        }

        [NotDecorated(typeof(DisposableDecorator))]
        public void NotDecoratedByDisposableDecorator()
        {
        }

        [NotDecorated(typeof(HostedServiceDecorator))]
        public void NotDecoratedByHostedServiceDecorator()
        {
        }

        [NotDecorated(typeof(LoggingDecorator<>))]
        public void NotDecoratedByLoggingDecorator()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        [NotDecorated]
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
        }
    }
}
