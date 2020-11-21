using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.AddImplementation
{
    public static class AddWithoutImplementationServiceCollectionExtensions
    {
        public static void AddSingletonWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithoutImplementation(x => x.AddSingleton<TService, TImplementation>());
        }

        public static void AddSingletonWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection, 
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithoutImplementation(x => x.AddSingleton<TService, TImplementation>(implementationFactory));
        }

        public static void AddSingletonWithoutImplementation<TService>(this IServiceCollection serviceCollection,
            TService instance)
            where TService : class
        {
            serviceCollection.AddSingletonWithoutImplementation(x => x.AddSingleton(instance));
        }

        public static void AddScopedWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithoutImplementation(x => x.AddScoped<TService, TImplementation>());
        }

        public static void AddScopedWithoutImplementation<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingletonWithoutImplementation(x => x.AddScoped<TService, TImplementation>(implementationFactory));
        }

        private static void AddSingletonWithoutImplementation(this IServiceCollection serviceCollection,
            Action<IServiceCollection> addAction)
        {
            if (serviceCollection is ServiceCollectionExtender extender && extender.Extensions.OfType<NotTransientImplementationExtension>().Any())
            {
                var filteredExtensions = extender.Extensions.Where(x => !(x is NotTransientImplementationExtension));
                extender.Add(CreateServiceDescriptor(addAction), filteredExtensions);
            }
            else
            {
                addAction(serviceCollection);
            }
        }

        private static ServiceDescriptor CreateServiceDescriptor(Action<IServiceCollection> addAction)
        {
            var factoryCollection = new SimpleServiceCollection();
            addAction(factoryCollection);

            return factoryCollection.Single();
        }
    }
}
