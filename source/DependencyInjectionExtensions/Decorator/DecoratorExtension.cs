using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

            if (!serviceDescriptor.ServiceType.IsInterface || !DecoratorFactory.CanDecorate(serviceDescriptor.ServiceType))
                return;

            //We need the already existent entry so we can decorate any number of times 
            var alreadyRegisterDescriptor = serviceCollection.Last(x => x.ServiceType == serviceDescriptor.ServiceType);

            ServiceDescriptor newServiceDescriptor;
            if (alreadyRegisterDescriptor.ImplementationInstance != null)
            {
                newServiceDescriptor = new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType,
                    provider => CreateDecorated(alreadyRegisterDescriptor.ImplementationInstance,
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

                newServiceDescriptor = new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType,
                    CreateFromType(implementationType, alreadyRegisterDescriptor.ServiceType),
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

            var createDecoratorMethod = typeof(DecoratorExtension).GetMethod(nameof(CreateDecorated), BindingFlags.NonPublic | BindingFlags.Instance);
            var callCreateDecorator = Expression.Call(Expression.Constant(this),
                createDecoratorMethod!,
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
            var callImplementation = Expression.Call(inputParameter, getServiceMethod!, Expression.Constant(implementationType));

            var createDecoratorMethod = typeof(DecoratorExtension).GetMethod(nameof(CreateDecorated), BindingFlags.NonPublic | BindingFlags.Instance);
            var callCreateDecorator = Expression.Call(Expression.Constant(this),
                createDecoratorMethod!,
                callImplementation,
                Expression.Constant(serviceType),
                inputParameter);

            var lambdaExpression = Expression.Lambda(callCreateDecorator, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        private object CreateDecorated(object toDecorate, Type decoratingType, IServiceProvider serviceProvider)
        {
            var decorated = DecoratorFactory.CreateDecorated(toDecorate, decoratingType, serviceProvider);

            var interfaces = toDecorate.GetType().GetInterfaces();
            var areImplemented = decorated.GetType().GetInterfaces().ToList();
            if (interfaces.All(x => areImplemented.Contains(x)))
            {
                return decorated;
            }

            var dynamicType = DynamicTypeCreator.CreateDynamicInterface(interfaces, toDecorate.GetType().Name);

            var methodInfo = typeof(InterfaceEnsurerDecorator).GetMethod(nameof(InterfaceEnsurerDecorator.Create));
            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

            return methodInfo.MakeGenericMethod(dynamicType).Invoke(null, new[] { toDecorate, decorated })!;
        }
    }
}
