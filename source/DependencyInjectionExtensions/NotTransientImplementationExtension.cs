using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    /// <summary>
    /// When register a service the implementation is not registered.
    /// But in some cases one need to access the implementation as well.
    /// Then it should be the same instance if registered as not transient
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

            serviceCollection.Add(new ServiceDescriptor(typeArguments[1],
                serviceDescriptor.ImplementationFactory,
                serviceDescriptor.Lifetime));

            serviceCollection.Add(new ServiceDescriptor(serviceDescriptor.ServiceType,
                ReflectionHelper.CreateImplementationFactory(typeArguments[1]), serviceDescriptor.Lifetime));
        }

        private static void AddWithType(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            serviceCollection.Add(new ServiceDescriptor(serviceDescriptor.ImplementationType,
                serviceDescriptor.ImplementationType,
                serviceDescriptor.Lifetime));

            var factory = ReflectionHelper.CreateImplementationFactory(serviceDescriptor.ImplementationType);

            serviceCollection.Add(new ServiceDescriptor(serviceDescriptor.ServiceType, factory, serviceDescriptor.Lifetime));
        }
    }
}
