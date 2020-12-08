using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DependencyInjectionExtensions.Decorator
{
    public class DecoratorImplementationFactoryBuilder
    {
        private readonly IDecoratorFactory _decoratorFactory;

        public DecoratorImplementationFactoryBuilder(IDecoratorFactory decoratorFactory)
        {
            _decoratorFactory = decoratorFactory;
        }

        public Func<IServiceProvider, object> CreateFromInstance(object instance, Type serviceType)
        {
            return provider => CreateDecorated(instance, serviceType, provider);
        }

        public Func<IServiceProvider, object> CreateFromImplementationFactory(Func<IServiceProvider, object> implementationFactory, Type serviceType)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            Expression<Func<IServiceProvider, object>> implementationFactoryExpression = provider => implementationFactory(provider);
            var invokedImplementationFactory = Expression.Invoke(implementationFactoryExpression, inputParameter);

            var createDecoratorMethod = typeof(DecoratorImplementationFactoryBuilder).GetMethod(nameof(CreateDecorated), BindingFlags.NonPublic | BindingFlags.Instance);
            var callCreateDecorator = Expression.Call(Expression.Constant(this),
                createDecoratorMethod!,
                invokedImplementationFactory,
                Expression.Constant(serviceType),
                inputParameter);

            var lambdaExpression = Expression.Lambda(callCreateDecorator, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        public Func<IServiceProvider, object> CreateFromType(Type implementationType, Type serviceType)
        {
            return CreateFromImplementationFactory(provider => provider.GetService(implementationType), serviceType);
        }

        private object CreateDecorated(object toDecorate, Type decoratingType, IServiceProvider serviceProvider)
        {
            var decorated = _decoratorFactory.CreateDecorated(toDecorate, decoratingType, serviceProvider);

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
