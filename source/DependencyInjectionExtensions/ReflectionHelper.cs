using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public static class ReflectionHelper
    {
        public static Func<IServiceProvider, object> CreateImplementationFactory(Type implementationType)
        {
            var inputParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var methodInfo = typeof(ServiceProviderServiceExtensions)
                .GetMethod(nameof(ServiceProviderServiceExtensions.GetService))?
                .MakeGenericMethod(implementationType);

            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

            var function = Expression.Call(null, methodInfo, inputParameter);

            var lambdaExpression = Expression.Lambda(function, inputParameter);

            return (Func<IServiceProvider, object>)lambdaExpression.Compile();
        }
    }
}
