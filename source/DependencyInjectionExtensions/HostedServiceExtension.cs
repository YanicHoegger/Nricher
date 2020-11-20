using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionExtensions
{
    /// <summary>
    /// When register a singleton service that implements the <see cref="IHostedService"/> interface,
    /// this extension adds a <see cref="IHostedService"/> service to the <see cref="IServiceCollection"/>
    /// </summary>
    public class HostedServiceExtension : IServiceCollectionExtension
    {
        private readonly Type _hostedServiceType = typeof(IHostedService);

        public void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            if (serviceDescriptor.Lifetime != ServiceLifetime.Singleton)
                return;

            ServiceDescriptor hostedServiceDescriptor;
            if (serviceDescriptor.ImplementationInstance != null)
            {
                if (!IsHostedService(serviceDescriptor.ImplementationInstance.GetType()))
                    return;

                hostedServiceDescriptor = new ServiceDescriptor(_hostedServiceType, serviceDescriptor.ImplementationInstance);
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                var typeArguments = serviceDescriptor.ImplementationFactory.GetType().GenericTypeArguments;
                if (!IsHostedService(typeArguments[1]))
                    return;

                hostedServiceDescriptor = CreateImplementationFactoryServiceDescriptor(serviceDescriptor, typeArguments[1], serviceCollection);
            }
            else
            {
                if (!IsHostedService(serviceDescriptor.ImplementationType))
                    return;

                hostedServiceDescriptor = CreateImplementationFactoryServiceDescriptor(serviceDescriptor, serviceDescriptor.ImplementationType, serviceCollection);
            }

            serviceCollection.Add(hostedServiceDescriptor);
        }

        private bool IsHostedService(Type type)
        {
            return _hostedServiceType.IsAssignableFrom(type);
        }

        private ServiceDescriptor CreateImplementationFactoryServiceDescriptor(ServiceDescriptor serviceDescriptor,
            Type type, 
            IServiceCollection serviceCollection)
        {
            var factory = CreateImplementationFactory(serviceDescriptor.ServiceType, type, serviceCollection);
            return new ServiceDescriptor(_hostedServiceType, factory, ServiceLifetime.Singleton);
        }

        private static Func<IServiceProvider, object> CreateImplementationFactory(Type serviceType, Type implementationType, IServiceCollection serviceCollection)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var getServicesMethodInfo = typeof(ServiceProviderServiceExtensions)
                .GetMethod(nameof(ServiceProviderServiceExtensions.GetServices), new[] { typeof(IServiceProvider) })?
                .MakeGenericMethod(serviceType);

            Debug.Assert(getServicesMethodInfo != null, nameof(getServicesMethodInfo) + " != null");

            var getServicesFunction = Expression.Call(null, getServicesMethodInfo, inputParameter);

            var skipMethodInfo = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.Skip))
                ?.MakeGenericMethod(serviceType);

            var skipCount = Expression.Constant(GetLastIndexOfSameServiceDescriptor(serviceType, implementationType, serviceCollection));
            var skipFunction = Expression.Call(null, skipMethodInfo!, getServicesFunction, skipCount);

            var getImplementationMethodInfo = typeof(Enumerable)
                .GetMethods()
                .Where(x => x.Name.Equals(nameof(Enumerable.First)))
                .Select(x => x.MakeGenericMethod(serviceType))
                .Single(x => x.GetParameters().Any(y => y.ParameterType == typeof(Func<,>).MakeGenericType(serviceType, typeof(bool))));

            var getTypeExpression = CreateGetTypeExpression(serviceType, implementationType);
            var getImplementationFunction = Expression.Call(null, getImplementationMethodInfo, skipFunction, getTypeExpression);

            var castExpression = Expression.Convert(getImplementationFunction, implementationType);

            var lambdaExpression = Expression.Lambda(castExpression, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        private static LambdaExpression CreateGetTypeExpression(Type serviceType, Type implementationType)
        {
            var inputParameter = Expression.Parameter(serviceType, "service");

            var getTypeMethodInfo = typeof(object).GetMethod(nameof(GetType));
            var getTypeFunction = Expression.Call(inputParameter, getTypeMethodInfo!);

            var equalExpression = Expression.Equal(getTypeFunction, Expression.Constant(implementationType));

            return Expression.Lambda(equalExpression, inputParameter);
        }

        private static int GetLastIndexOfSameServiceDescriptor(Type serviceType, 
            Type implementationType,
            IServiceCollection serviceCollection)
        {
            return serviceCollection.Count(x => x.ServiceType == serviceType &&
                                                                x.GetImplementationType() == implementationType) - 1;
        }
    }
}
