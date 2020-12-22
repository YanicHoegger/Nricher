using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Decorators.Tests
{
    public class HostedService : IHostedService, ISomeAction
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void SomeAction()
        {
            //Nothing to do here
        }
    }
}
