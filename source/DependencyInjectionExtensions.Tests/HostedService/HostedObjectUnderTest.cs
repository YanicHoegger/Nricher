using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionExtensions.Tests.HostedService
{
    public class HostedObjectUnderTest : IObjectUnderTest, IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public interface IHostedServiceUnderTest : IObjectUnderTest, IHostedService
    {
    }

    public class HostedServiceUnderTest : HostedObjectUnderTest,  IHostedServiceUnderTest
    {
    }
}
