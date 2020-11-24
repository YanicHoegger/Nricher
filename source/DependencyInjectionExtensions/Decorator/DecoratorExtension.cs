using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.Decorator
{
    public class DecoratorExtension : IServiceCollectionExtension
    {
        public DecoratorExtension(IDecoratorFactory decoratorFactory)
        {
            DecoratorFactory = decoratorFactory;
        }

        public IDecoratorFactory DecoratorFactory { get; }

        public void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            if (serviceDescriptor == null) 
                throw new ArgumentNullException(nameof(serviceDescriptor));
            if (serviceCollection == null) 
                throw new ArgumentNullException(nameof(serviceCollection));

            if (!DecoratorFactory.CanDecorate(serviceDescriptor.ServiceType))
                return;

            //We need the already existent entry so we can decorate any number of times 
            var alreadyRegisterDescriptor = serviceCollection.Last(x => x.ServiceType == serviceDescriptor.ServiceType);

            ServiceDescriptor newServiceDescriptor;
            if (alreadyRegisterDescriptor.ImplementationInstance != null)
            {
                newServiceDescriptor = new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType,
                    provider => DecoratorFactory.CreateDecorated(alreadyRegisterDescriptor.ImplementationInstance,
                        alreadyRegisterDescriptor.ServiceType, provider),
                    alreadyRegisterDescriptor.Lifetime);
            }
            else if (alreadyRegisterDescriptor.ImplementationFactory != null)
            {
                newServiceDescriptor = new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType,
                    CreateFromImplementationFactory(alreadyRegisterDescriptor.ImplementationFactory, serviceDescriptor.ServiceType),
                    alreadyRegisterDescriptor.Lifetime);
            }
            else
            {
                if (serviceCollection.All(x => x.ServiceType != alreadyRegisterDescriptor.ImplementationType))
                {
                    serviceCollection.Add(new ServiceDescriptor(alreadyRegisterDescriptor.ImplementationType, alreadyRegisterDescriptor.ImplementationType, alreadyRegisterDescriptor.Lifetime));
                }

                newServiceDescriptor = new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType, 
                    CreateFromType(alreadyRegisterDescriptor.ImplementationType, alreadyRegisterDescriptor.ServiceType), 
                    alreadyRegisterDescriptor.Lifetime);
            }

            ServiceCollectionHelper.ReplaceServiceDescriptor(newServiceDescriptor, serviceCollection);
        }

        /// <param name="implementationFactory"></param>
        /// <param name="serviceType"></param>
        /// <returns>serviceProvider =&gt; <see cref="DecoratorFactory"/>.<see cref="IDecoratorFactory.CreateDecorated"/>(<paramref name="implementationFactory"/>(), <paramref name="serviceType"/>, serviceProvider)</returns>
        private Func<IServiceProvider, object> CreateFromImplementationFactory(Func<IServiceProvider, object> implementationFactory, Type serviceType)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            Expression<Func<IServiceProvider, object>> implementationFactoryExpression = provider => implementationFactory(provider);
            var invokedImplementationFactory = Expression.Invoke(implementationFactoryExpression, inputParameter);

            var createDecoratorMethod = typeof(IDecoratorFactory).GetMethod(nameof(IDecoratorFactory.CreateDecorated));
            Debug.Assert(createDecoratorMethod != null, nameof(createDecoratorMethod) + " != null");
            var callCreateDecorator = Expression.Call(Expression.Constant(DecoratorFactory),
                createDecoratorMethod,
                invokedImplementationFactory,
                Expression.Constant(serviceType),
                inputParameter);

            var lambdaExpression = Expression.Lambda(callCreateDecorator, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        /// <param name="implementationType"></param>
        /// <param name="serviceType"></param>
        /// <returns>serviceProvider =&gt; <see cref="DecoratorFactory"/>.<see cref="IDecoratorFactory.CreateDecorated"/>(serviceProvider.GetService(<paramref name="implementationType"/>), <paramref name="serviceType"/>, serviceProvider)</returns>
        private Func<IServiceProvider, object> CreateFromType(Type implementationType, Type serviceType)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService));
            Debug.Assert(getServiceMethod != null, nameof(getServiceMethod) + " != null");
            var callImplementation = Expression.Call(inputParameter, getServiceMethod, Expression.Constant(implementationType));

            var createDecoratorMethod = typeof(IDecoratorFactory).GetMethod(nameof(IDecoratorFactory.CreateDecorated));
            Debug.Assert(createDecoratorMethod != null, nameof(createDecoratorMethod) + " != null");
            var callCreateDecorator = Expression.Call(Expression.Constant(DecoratorFactory),
                createDecoratorMethod,
                callImplementation,
                Expression.Constant(serviceType),
                inputParameter);

            var lambdaExpression = Expression.Lambda(callCreateDecorator, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }
    }
}
