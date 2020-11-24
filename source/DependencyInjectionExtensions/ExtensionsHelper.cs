using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public static class ExtensionsHelper
    {
        public static void AddWithoutExtension<T>(this IServiceCollection serviceCollection,
            Action<IServiceCollection> addAction)
            where T : IServiceCollectionExtension
        {
            if (serviceCollection is ServiceCollectionExtender extender && extender.Extensions.OfType<T>().Any())
            {
                var filteredExtensions = extender.Extensions.Where(x => !(x is T));
                extender.Add(CreateServiceDescriptor(addAction), filteredExtensions);
            }
            else
            {
                addAction(serviceCollection);
            }
        }

        public static void Add<T>(this IServiceCollection serviceCollection,
            Action<IServiceCollection> addAction)
            where T : IServiceCollectionExtension, new()
        {
            if (serviceCollection is ServiceCollectionExtender extender && extender.Extensions.OfType<T>().Any())
            {
                addAction(serviceCollection);
            }
            else
            {
                var newExtender = new ServiceCollectionExtender(serviceCollection, new IServiceCollectionExtension[] { new T() });
                addAction(newExtender);
            }
        }

        private static ServiceDescriptor CreateServiceDescriptor(Action<IServiceCollection> addAction)
        {
            var factoryCollection = new SimpleServiceCollection();
            addAction(factoryCollection);

            return factoryCollection.Single();
        }

        private class SimpleServiceCollection : List<ServiceDescriptor>, IServiceCollection
        {
        }
    }
}
