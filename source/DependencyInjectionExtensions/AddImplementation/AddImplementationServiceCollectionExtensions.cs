using System;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.AddImplementation
{
    public static class AddImplementationServiceCollectionExtensions
    {
        public static void AddSingletonWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.Add<NotTransientImplementationExtension>(x => x.AddSingleton<TService, TImplementation>());
        }

        public static void AddSingletonWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.Add<NotTransientImplementationExtension>(x => x.AddSingleton<TService, TImplementation>(implementationFactory));
        }

        public static void AddSingletonWithImplementation<TService>(this IServiceCollection serviceCollection,
            TService instance)
            where TService : class
        {
            serviceCollection.Add<NotTransientImplementationExtension>(x => x.AddSingleton(instance));
        }

        public static void AddScopedWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.Add<NotTransientImplementationExtension>(x => x.AddScoped<TService, TImplementation>());
        }

        public static void AddScopedWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.Add<NotTransientImplementationExtension>(x => x.AddScoped<TService, TImplementation>(implementationFactory));
        }
    }
}
