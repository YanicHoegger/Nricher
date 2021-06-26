using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nricher.DependencyInjectionExtensions.HostedService
{
    public static class AddHostedServiceServiceCollectionExtensions
    {
        public static void AddSingletonHostedService<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            serviceCollection.Add<HostedServiceExtension>(x => x.AddSingleton<TService, TImplementation>());
        }

        public static void AddSingletonHostedService<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            serviceCollection.Add<HostedServiceExtension>(x => x.AddSingleton<TService, TImplementation>(implementationFactory));
        }

        public static void AddSingletonHostedService<TService, TImplementation>(this IServiceCollection serviceCollection,
            TImplementation service)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            serviceCollection.Add<HostedServiceExtension>(x => x.AddSingleton<TService>(service));
        }
    }
}
