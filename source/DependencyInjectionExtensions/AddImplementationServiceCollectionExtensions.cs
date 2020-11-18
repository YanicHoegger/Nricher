using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public static class AddImplementationServiceCollectionExtensions
    {
        public static void AddSingletonWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithImplementation(x => x.AddSingleton<TService, TImplementation>());
        }

        public static void AddSingletonWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithImplementation(x => x.AddSingleton<TService, TImplementation>(implementationFactory));
        }

        public static void AddSingletonWithImplementation<TService>(this IServiceCollection serviceCollection,
            TService instance)
            where TService : class
        {
            serviceCollection.AddSingletonWithImplementation(x => x.AddSingleton(instance));
        }

        public static void AddScopedWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithImplementation(x => x.AddScoped<TService, TImplementation>());
        }

        public static void AddScopedWithImplementation<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithImplementation(x => x.AddScoped<TService, TImplementation>(implementationFactory));
        }

        private static void AddSingletonWithImplementation(this IServiceCollection serviceCollection,
            Action<IServiceCollection> addAction)
        {
            if (serviceCollection is ServiceCollectionExtender extender && extender.Extensions.OfType<NotTransientImplementationExtension>().Any())
            {
                addAction(serviceCollection);
            }
            else
            {
                var newExtender = new ServiceCollectionExtender(serviceCollection, new[] { new NotTransientImplementationExtension() });
                addAction(newExtender);
            }
        }
    }
}
