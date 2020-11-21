using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.HostedService
{
    public class ImplementationFactoryBuilder
    {
        private readonly Type _serviceType;
        private readonly Type _implementationType;
        private readonly IServiceCollection _serviceCollection;

        public ImplementationFactoryBuilder(Type serviceType, Type implementationType, IServiceCollection serviceCollection)
        {
            _serviceType = serviceType;
            _implementationType = implementationType;
            _serviceCollection = serviceCollection;
        }

        public Func<IServiceProvider, object> CreateImplementationFactory()
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var methodBody = CreateMethodBody(inputParameter);

            var lambdaExpression = Expression.Lambda(methodBody, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }

        private UnaryExpression CreateMethodBody(Expression inputParameter)
        {
            var getServicesExpression = GetServices(inputParameter);
            var skipExpression = Skip(getServicesExpression);
            var getImplementationExpression = GetImplementation(skipExpression);

            return Expression.Convert(getImplementationExpression, _implementationType);
        }

        private MethodCallExpression GetServices(Expression inputParameter)
        {
            var methodInfo = typeof(ServiceProviderServiceExtensions)
                .GetMethod(nameof(ServiceProviderServiceExtensions.GetServices), new[] { typeof(IServiceProvider) })?
                .MakeGenericMethod(_serviceType);

            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

            return Expression.Call(null, methodInfo, inputParameter);
        }

        private MethodCallExpression GetImplementation(Expression skipFunction)
        {
            var methodInfo = typeof(Enumerable)
                .GetMethods()
                .Where(x => x.Name.Equals(nameof(Enumerable.First)))
                .Select(x => x.MakeGenericMethod(_serviceType))
                .Single(x =>
                    x.GetParameters().Any(y => y.ParameterType == typeof(Func<,>).MakeGenericType(_serviceType, typeof(bool))));

            var getTypeExpression = CreateGetTypeExpression();

            return Expression.Call(null, methodInfo, skipFunction, getTypeExpression);
        }

        private LambdaExpression CreateGetTypeExpression()
        {
            var inputParameter = Expression.Parameter(_serviceType, "service");

            var getTypeMethodInfo = typeof(object).GetMethod(nameof(GetType));
            var getTypeFunction = Expression.Call(inputParameter, getTypeMethodInfo!);

            var equalExpression = Expression.Equal(getTypeFunction, Expression.Constant(_implementationType));

            return Expression.Lambda(equalExpression, inputParameter);
        }

        private MethodCallExpression Skip(Expression getServicesFunction)
        {
            var methodInfo = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.Skip))
                ?.MakeGenericMethod(_serviceType);

            var skipCount = Expression.Constant(GetSkipCount());

            return Expression.Call(null, methodInfo!, getServicesFunction, skipCount);
        }

        private int GetSkipCount()
        {
            var count = _serviceCollection.Count(x => x.ServiceType == _serviceType &&
                                                     x.GetImplementationType() == _implementationType);
            return count - 1;
        }
    }
}
