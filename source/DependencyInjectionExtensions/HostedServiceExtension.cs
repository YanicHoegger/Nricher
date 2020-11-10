using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionExtensions
{
    public class HostedServiceExtension : IServiceCollectionExtension
    {
        private readonly Type _hostedServiceType = typeof(IHostedService);

        public void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
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

                hostedServiceDescriptor = CreateImplementationFactoryServiceDescriptor(serviceDescriptor, serviceCollection, typeArguments[1]);
            }
            else
            {
                if (!IsHostedService(serviceDescriptor.ImplementationType))
                    return;

                hostedServiceDescriptor = CreateImplementationFactoryServiceDescriptor(serviceDescriptor, serviceCollection, serviceDescriptor.ImplementationType);
            }

            serviceCollection.TryAddEnumerable(hostedServiceDescriptor);
        }

        private bool IsHostedService(Type type)
        {
            return _hostedServiceType.IsAssignableFrom(type);
        }

        private ServiceDescriptor CreateImplementationFactoryServiceDescriptor(ServiceDescriptor serviceDescriptor, 
            IServiceCollection serviceCollection, 
            Type type)
        {
            if (serviceCollection.All(x => x.ServiceType != type))
            {
                if (serviceDescriptor.Lifetime == ServiceLifetime.Transient)
                {
                    serviceCollection.Add(new ServiceDescriptor(type, type, ServiceLifetime.Transient));
                }
                else
                {
                    new NotTransientImplementationExtension().Extend(serviceDescriptor, serviceCollection);
                }
            }

            var factory = ReflectionHelper.CreateImplementationFactory(type);

            return new ServiceDescriptor(_hostedServiceType, factory, ServiceLifetime.Singleton);
        }
    }
}
