using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nricher.DependencyInjectionExtensions.AddImplementation
{
    public static class AddWithoutImplementationServiceCollectionExtensions
    {
        public static void AddSingletonWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<NotTransientImplementationExtension>(x => x.AddSingleton<TService, TImplementation>());
        }

        public static void AddSingletonWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection, 
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<NotTransientImplementationExtension>(x => x.AddSingleton<TService, TImplementation>(implementationFactory));
        }

        public static void AddSingletonWithoutImplementation<TService>(this IServiceCollection serviceCollection,
            TService instance)
            where TService : class
        {
            serviceCollection.AddWithoutExtension<NotTransientImplementationExtension>(x => x.AddSingleton(instance));
        }

        public static void AddScopedWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<NotTransientImplementationExtension>(x => x.AddScoped<TService, TImplementation>());
        }

        public static void AddScopedWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<NotTransientImplementationExtension>(x => x.AddScoped<TService, TImplementation>(implementationFactory));
        }
    }
}
