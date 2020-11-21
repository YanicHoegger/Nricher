using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionExtensions.HostedService
{
    public static class AddWithoutHostedServiceServiceCollectionExtensions
    {
        public static void AddSingletonWithoutHostedService<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            serviceCollection.AddWithoutExtension<HostedServiceExtension>(x => x.AddSingleton<TService, TImplementation>());
        }

        public static void AddSingletonWithoutHostedService<TService, TImplementation>(this IServiceCollection serviceCollection, 
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            serviceCollection.AddWithoutExtension<HostedServiceExtension>(x => x.AddSingleton<TService, TImplementation>(implementationFactory));
        }

        public static void AddSingletonWithoutHostedService<TService, TImplementation>(this IServiceCollection serviceCollection,
            TImplementation service)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            serviceCollection.AddWithoutExtension<HostedServiceExtension>(x => x.AddSingleton<TService>(service));
        }
    }
}
