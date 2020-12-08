using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.Decorator
{
    public class DecoratorExtension : IServiceCollectionExtension
    {
        private readonly ImplementationFactoryFactory _factory;

        public DecoratorExtension(IDecoratorFactory decoratorFactory)
        {
            DecoratorFactory = decoratorFactory;
            _factory = new ImplementationFactoryFactory(decoratorFactory);
        }

        public IDecoratorFactory DecoratorFactory { get; }

        public void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            if (serviceDescriptor == null)
                throw new ArgumentNullException(nameof(serviceDescriptor));
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (!serviceDescriptor.ServiceType.IsInterface || !DecoratorFactory.CanDecorate(serviceDescriptor.ServiceType))
                return;

            //We need the already existent entry so we can decorate any number of times 
            var alreadyRegisterDescriptor = serviceCollection.Last(x => x.ServiceType == serviceDescriptor.ServiceType);

            Func<IServiceProvider, object> implementationFactory;
            if (alreadyRegisterDescriptor.ImplementationInstance != null)
            {
                implementationFactory = _factory.CreateFromInstance(alreadyRegisterDescriptor.ImplementationInstance,
                    alreadyRegisterDescriptor.ServiceType);
            }
            else if (alreadyRegisterDescriptor.ImplementationFactory != null)
            {
                implementationFactory = _factory.CreateFromImplementationFactory(alreadyRegisterDescriptor.ImplementationFactory,
                    serviceDescriptor.ServiceType);
            }
            else
            {
                Debug.Assert(alreadyRegisterDescriptor.ImplementationType != null, "alreadyRegisterDescriptor.ImplementationType != null");

                Type implementationType;
                if (alreadyRegisterDescriptor.ImplementationType.IsSealed)
                {
                    if(serviceCollection.All(x => x.ServiceType != alreadyRegisterDescriptor.ImplementationType))
                        throw new InvalidOperationException($"Sealed type {alreadyRegisterDescriptor.ImplementationType.Name} need to be added to the service collection");

                    implementationType = alreadyRegisterDescriptor.ImplementationType;
                }
                else
                {
                    implementationType = DynamicTypeCreator.CreateInheritedType(alreadyRegisterDescriptor.ImplementationType);
                    serviceCollection.Add(new ServiceDescriptor(implementationType, implementationType, alreadyRegisterDescriptor.Lifetime));
                }

                implementationFactory = _factory.CreateFromType(implementationType, alreadyRegisterDescriptor.ServiceType);
            }

            var newServiceDescriptor = new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType,
                implementationFactory, 
                alreadyRegisterDescriptor.Lifetime);

            ServiceCollectionHelper.ReplaceServiceDescriptor(newServiceDescriptor, serviceCollection);
        }
    }
}
