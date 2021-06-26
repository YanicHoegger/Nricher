using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nricher.DependencyInjectionExtensions.HostedService
{
    public class ImplementationFactoryBuilder
    {
        private readonly Type _serviceType;
        private readonly IServiceCollection _serviceCollection;

        public ImplementationFactoryBuilder(Type serviceType, IServiceCollection serviceCollection)
        {
            _serviceType = serviceType;
            _serviceCollection = serviceCollection;
        }

        public Func<IServiceProvider, IHostedService> CreateImplementationFactory()
        {
            var skipCount = GetSkipCount();
            return provider => provider.GetServices(_serviceType).Cast<IHostedService>().Skip(skipCount).First();
        }

        private int GetSkipCount()
        {
            var count = _serviceCollection.Count(x => x.ServiceType == _serviceType &&
                                                     typeof(IHostedService).IsAssignableFrom(x.GetImplementationType()));
            return count - 1;
        }
    }
}
