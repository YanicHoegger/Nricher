using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public class DecoratorExtension : IServiceCollectionExtension
    {
        private readonly IDecoratorFactory _decoratorFactory;

        //TODO: remove
        public static event Action<IServiceCollection> Temp;

        public DecoratorExtension(IDecoratorFactory decoratorFactory)
        {
            _decoratorFactory = decoratorFactory;
        }

        public void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            if (!serviceDescriptor.ServiceType.IsInterface)
                return;

            //We need the already existent entry so we can decorate any number of times 
            var alreadyRegisterDescriptor = serviceCollection.Last(x => x.ServiceType == serviceDescriptor.ServiceType);

            if (alreadyRegisterDescriptor.ImplementationInstance != null)
            {
                ReplaceServiceDescriptor(new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType,
                    provider => _decoratorFactory.CreateDecorated(alreadyRegisterDescriptor.ImplementationInstance, alreadyRegisterDescriptor.ServiceType, provider),
                    alreadyRegisterDescriptor.Lifetime), serviceCollection);
            }
            else if (alreadyRegisterDescriptor.ImplementationFactory != null)
            {
                var containerType =
                    typeof(DecoratorContainer<,>).MakeGenericType(alreadyRegisterDescriptor.ServiceType, _decoratorFactory.GetType());

                var containerFromImplementationFactory = CreateContainerFromImplementationFactory(alreadyRegisterDescriptor.ImplementationFactory, alreadyRegisterDescriptor.ServiceType);
                serviceCollection.Add(new ServiceDescriptor(containerType, containerFromImplementationFactory, serviceDescriptor.Lifetime));

                ReplaceServiceDescriptor(new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType,
                    CreateServiceImplementationFactory(containerType),
                    alreadyRegisterDescriptor.Lifetime), serviceCollection);

                Temp?.Invoke(serviceCollection);
            }
            else
            {
                var containerType =
                    typeof(DecoratorContainer<,>).MakeGenericType(alreadyRegisterDescriptor.ServiceType, _decoratorFactory.GetType());

                if (serviceCollection.All(x => x.ServiceType != serviceDescriptor.ImplementationType))
                {
                    serviceCollection.Add(new ServiceDescriptor(serviceDescriptor.ImplementationType, serviceDescriptor.ImplementationType, serviceDescriptor.Lifetime));
                }

                var containerFromTypeFactory = CreateContainerFromTypeFactory(alreadyRegisterDescriptor.ServiceType, alreadyRegisterDescriptor.ImplementationType);
                serviceCollection.Add(new ServiceDescriptor(containerType, containerFromTypeFactory, serviceDescriptor.Lifetime));

                ReplaceServiceDescriptor(new ServiceDescriptor(alreadyRegisterDescriptor.ServiceType, CreateServiceImplementationFactory(containerType), alreadyRegisterDescriptor.Lifetime), serviceCollection);
            }
        }

        private Func<IServiceProvider, object> CreateFromImplementationFactory(Func<IServiceProvider, object> implementationFactory, Type serviceType)
        {
        }

        /// <param name="implementationFactory"></param>
        /// <param name="serviceType"></param>
        /// <returns>serviceProvider =&gt; new DecoratorContainer{<paramref name="serviceType"/>, TDecorator}(<see cref="_decoratorFactory"/>.<see cref="IDecoratorFactory.CreateDecorated"/>(<paramref name="implementationFactory"/>, <paramref name="serviceType"/>, serviceProvider))</returns>
        private Func<IServiceProvider, object> CreateContainerFromImplementationFactory(Func<IServiceProvider, object> implementationFactory, Type serviceType)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            Expression<Func<IServiceProvider, object>> implementationFactoryExpression = provider => implementationFactory(provider);
            var invokedImplementationFactory = Expression.Invoke(implementationFactoryExpression, inputParameter);

            var createDecoratorMethod = typeof(IDecoratorFactory).GetMethod(nameof(IDecoratorFactory.CreateDecorated));
            var callCreateDecorator = Expression.Call(Expression.Constant(_decoratorFactory),
                createDecoratorMethod,
                invokedImplementationFactory,
                Expression.Constant(serviceType),
                inputParameter);

            var cast = Expression.Convert(callCreateDecorator, serviceType);

            var constructorInfo = typeof(DecoratorContainer<,>).MakeGenericType(serviceType, _decoratorFactory.GetType()).GetConstructor(new[] { serviceType });
            var memberInit = Expression.New(constructorInfo, cast);

            var lambdaExpression = Expression.Lambda(memberInit, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns>serviceProvider =&gt; new DecoratorContainer{serviceType, TDecorator}(serviceProvider.GetService(serviceType))</returns>
        private Func<IServiceProvider, object> CreateContainerFromTypeFactory(Type serviceType, Type implementationType)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService));
            var callImplementation = Expression.Call(inputParameter, getServiceMethod, Expression.Constant(implementationType));

            var createDecoratorMethod = typeof(IDecoratorFactory).GetMethod(nameof(IDecoratorFactory.CreateDecorated));
            var callCreateDecorator = Expression.Call(Expression.Constant(_decoratorFactory),
                createDecoratorMethod,
                callImplementation,
                Expression.Constant(serviceType),
                inputParameter);

            var cast = Expression.Convert(callCreateDecorator, serviceType);

            var constructorInfo = typeof(DecoratorContainer<,>).MakeGenericType(serviceType, _decoratorFactory.GetType()).GetConstructor(new[] { serviceType });
            var memberInit = Expression.New(constructorInfo, cast);

            var lambdaExpression = Expression.Lambda(memberInit, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        /// <param name="decoratorContainerType"></param>
        /// <returns>serviceProvider =&gt; serviceProvider.GetService{<paramref name="decoratorContainerType"/>}().Decorated</returns>
        private Func<IServiceProvider, object> CreateServiceImplementationFactory(Type decoratorContainerType)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var methodInfo = typeof(ServiceProviderServiceExtensions)
                .GetMethod(nameof(ServiceProviderServiceExtensions.GetService))?
                .MakeGenericMethod(decoratorContainerType);
            var function = Expression.Call(null, methodInfo, inputParameter);

            var property = Expression.Property(function,
                decoratorContainerType.GetProperty(nameof(DecoratorContainer<object, object>.Decorated)));

            var lambdaExpression = Expression.Lambda(property, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        private void ReplaceServiceDescriptor(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            var toReplace = serviceCollection.Last(x => x.ServiceType == serviceDescriptor.ServiceType);
            var index = serviceCollection.IndexOf(toReplace);
            serviceCollection[index] = serviceDescriptor;
        }
    }
}
