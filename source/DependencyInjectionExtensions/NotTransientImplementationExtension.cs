using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    /// <summary>
    /// When register a service the implementation is not registered.
    /// But in some cases one need to access the implementation as well.
    /// Then it should be the same instance if registered as not transient.
    /// 
    /// This does not work if one is to add enumerable of the implementation type.
    /// If this behavior is wanted, then use the <see cref="AddWithoutImplementationServiceCollectionExtensions"/> extension method
    /// </summary>
    public class NotTransientImplementationExtension : IServiceCollectionExtension
    {
        public void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            if (serviceDescriptor.Lifetime.Equals(ServiceLifetime.Transient))
                return;

            if (serviceDescriptor.ImplementationInstance != null)
            {
                serviceCollection.Add(new ServiceDescriptor(serviceDescriptor.ImplementationInstance.GetType(), serviceDescriptor.ImplementationInstance));
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                AddWithImplementationFactory(serviceDescriptor, serviceCollection);
            }
            else
            {
                AddWithType(serviceDescriptor, serviceCollection);
            }
        }

        private static void AddWithImplementationFactory(ServiceDescriptor serviceDescriptor,
            IServiceCollection serviceCollection)
        {
            var typeArguments = serviceDescriptor.ImplementationFactory.GetType().GenericTypeArguments;

            if(typeArguments[1] == serviceDescriptor.ServiceType)
                return;

            serviceCollection.Add(new ServiceDescriptor(typeArguments[1],
                serviceDescriptor.ImplementationFactory,
                serviceDescriptor.Lifetime));

            var newServiceDescriptor = new ServiceDescriptor(serviceDescriptor.ServiceType,
                ReflectionHelper.CreateImplementationFactory(typeArguments[1]), 
                serviceDescriptor.Lifetime);

            ServiceCollectionHelper.ReplaceServiceDescriptor(newServiceDescriptor, serviceCollection);
        }

        private static void AddWithType(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            serviceCollection.Add(new ServiceDescriptor(serviceDescriptor.ImplementationType,
                serviceDescriptor.ImplementationType,
                serviceDescriptor.Lifetime));

            var factory = ReflectionHelper.CreateImplementationFactory(serviceDescriptor.ImplementationType);

            var newServiceDescriptor = new ServiceDescriptor(serviceDescriptor.ServiceType, factory, serviceDescriptor.Lifetime);
            ServiceCollectionHelper.ReplaceServiceDescriptor(newServiceDescriptor, serviceCollection);
        }
    }
}
