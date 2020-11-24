using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public static class ServiceCollectionHelper
    {
        public static void ReplaceServiceDescriptor(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            if (serviceDescriptor == null) 
                throw new ArgumentNullException(nameof(serviceDescriptor));
            if (serviceCollection == null) 
                throw new ArgumentNullException(nameof(serviceCollection));

            var toReplace = serviceCollection.Last(x => x.ServiceType == serviceDescriptor.ServiceType);
            var index = serviceCollection.IndexOf(toReplace);
            serviceCollection[index] = serviceDescriptor;
        }

        public static Type GetImplementationType(this ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor == null) 
                throw new ArgumentNullException(nameof(serviceDescriptor));

            if (serviceDescriptor.ImplementationType != null)
            {
                return serviceDescriptor.ImplementationType;
            }

            if (serviceDescriptor.ImplementationInstance != null)
            {
                return serviceDescriptor.ImplementationInstance.GetType();
            }

            if (serviceDescriptor.ImplementationFactory != null)
            {
                var typeArguments = serviceDescriptor.ImplementationFactory.GetType().GenericTypeArguments;

                Debug.Assert(typeArguments.Length == 2);

                return typeArguments[1];
            }

            Debug.Assert(false, "ImplementationType, ImplementationInstance or ImplementationFactory must be non null");
            return null;
        }
    }
}
