using System;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.Decorator
{
    public static class AddWithoutDecoratorServiceCollectionExtensions
    {
        public static void AddSingletonWithoutDecorator<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<DecoratorExtension>(x => x.AddSingleton<TService, TImplementation>());
        }

        public static void AddSingletonWithoutDecorator<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<DecoratorExtension>(x => x.AddSingleton<TService, TImplementation>(implementationFactory));
        }

        public static void AddSingletonWithoutDecorator<TService>(this IServiceCollection serviceCollection,
            TService instance)
            where TService : class
        {
            serviceCollection.AddWithoutExtension<DecoratorExtension>(x => x.AddSingleton(instance));
        }

        public static void AddScopedWithoutDecorator<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<DecoratorExtension>(x => x.AddScoped<TService, TImplementation>());
        }

        public static void AddScopedWithoutDecorator<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<DecoratorExtension>(x => x.AddScoped<TService, TImplementation>(implementationFactory));
        }

        public static void AddTransientWithoutDecorator<TService, TImplementation>(
            this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<DecoratorExtension>(x => x.AddTransient<TService, TImplementation>());
        }

        public static void AddTransientWithoutDecorator<TService, TImplementation>(this IServiceCollection serviceCollection,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddWithoutExtension<DecoratorExtension>(x => x.AddTransient<TService, TImplementation>(implementationFactory));
        }
    }
}
