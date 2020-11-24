using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.Decorator
{
    public static class AddDecoratorServiceCollectionExtensions
    {
        public static void AddDecoratedSingleton<TService, TImplementation>(this IServiceCollection serviceCollection, IDecoratorFactory decoratorFactory)
            where TService : class
            where TImplementation : class, TService
        {
            CheckFactory<TService>(decoratorFactory);
            serviceCollection.Add(x => x.AddSingleton<TService, TImplementation>(), decoratorFactory);
        }

        public static void AddDecoratedSingleton<TService, TImplementation>(this IServiceCollection serviceCollection, 
            Func<IServiceProvider, TImplementation> implementationFactory, 
            IDecoratorFactory decoratorFactory)
            where TService : class
            where TImplementation : class, TService
        {
            CheckFactory<TService>(decoratorFactory);
            serviceCollection.Add(x => x.AddSingleton<TService, TImplementation>(implementationFactory), decoratorFactory);
        }

        public static void AddDecoratedSingleton<TService>(this IServiceCollection serviceCollection,
            TService implementationInstance,
            IDecoratorFactory decoratorFactory)
            where TService : class
        {
            CheckFactory<TService>(decoratorFactory);
            serviceCollection.Add(x => x.AddSingleton(implementationInstance), decoratorFactory);
        }

        public static void AddDecoratedScoped<TService, TImplementation>(this IServiceCollection serviceCollection, IDecoratorFactory decoratorFactory)
            where TService : class
            where TImplementation : class, TService
        {
            CheckFactory<TService>(decoratorFactory);
            serviceCollection.Add(x => x.AddScoped<TService, TImplementation>(), decoratorFactory);
        }

        public static void AddDecoratedScoped<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory,
            IDecoratorFactory decoratorFactory)
            where TService : class
            where TImplementation : class, TService
        {
            CheckFactory<TService>(decoratorFactory);
            serviceCollection.Add(x => x.AddScoped<TService, TImplementation>(implementationFactory), decoratorFactory);
        }

        public static void AddDecoratedTransient<TService, TImplementation>(this IServiceCollection serviceCollection, IDecoratorFactory decoratorFactory)
            where TService : class
            where TImplementation : class, TService
        {
            CheckFactory<TService>(decoratorFactory);
            serviceCollection.Add(x => x.AddTransient<TService, TImplementation>(), decoratorFactory);
        }

        public static void AddDecoratedTransient<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory,
            IDecoratorFactory decoratorFactory)
            where TService : class
            where TImplementation : class, TService
        {
            CheckFactory<TService>(decoratorFactory);
            serviceCollection.Add(x => x.AddTransient<TService, TImplementation>(implementationFactory), decoratorFactory);
        }

        private static void CheckFactory<TService>(IDecoratorFactory decoratorFactory)
            where TService : class
        {
            if (decoratorFactory == null)
                throw new ArgumentNullException(nameof(decoratorFactory));
            if (!decoratorFactory.CanDecorate(typeof(TService)))
                throw new ArgumentException($"The {nameof(decoratorFactory)} should be able to decorate {typeof(TService).Name}");
        }

        private static void Add(this IServiceCollection serviceCollection,
            Action<IServiceCollection> addAction, IDecoratorFactory decoratorFactory)
        {
            if (serviceCollection is ServiceCollectionExtender extender
                && extender.Extensions.OfType<DecoratorExtension>().Any(x => x.DecoratorFactory.GetType() == decoratorFactory.GetType()))
            {
                addAction(serviceCollection);
            }
            else
            {
                var serviceCollectionExtensions = new IServiceCollectionExtension[] { new DecoratorExtension(decoratorFactory) };
                var newExtender = new ServiceCollectionExtender(serviceCollection, serviceCollectionExtensions);

                addAction(newExtender);
            }
        }
    }
}
