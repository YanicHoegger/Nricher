using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionExtensions.HostedService
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

            var implementationType = serviceDescriptor.GetImplementationType();
            if (!IsHostedService(implementationType)) 
                return;

            var hostedServiceDescriptor = CreateImplementationFactoryServiceDescriptor(serviceDescriptor, implementationType, serviceCollection);
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
            var implementationFactoryBuilder = new ImplementationFactoryBuilder(serviceDescriptor.ServiceType, type, serviceCollection);
            var factory = implementationFactoryBuilder.CreateImplementationFactory();

            return new ServiceDescriptor(_hostedServiceType, factory, ServiceLifetime.Singleton);
        }
    }
}
