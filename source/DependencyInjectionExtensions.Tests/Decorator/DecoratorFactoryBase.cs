using System;
using DependencyInjectionExtensions.Decorator;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public abstract class DecoratorFactoryBase : IDecoratorFactory
    {
        public DecoratorResult CreateDecorated(object toDecorate, Type decoratingType, IServiceProvider serviceProvider)
        {
            var decorated = DecoratorType
                                .MakeGenericType(decoratingType)
                                .GetMethod(MethodName)
                                ?.Invoke(null, new[] { toDecorate })
                            ?? throw new InvalidOperationException($"Could not find method {MethodName} in {DecoratorType}");

            return new DecoratorResult(decoratingType, decorated);
        }

        public bool CanDecorate(Type decoratingType) => true;

        protected abstract Type DecoratorType { get; }
        protected abstract string MethodName { get; }
    }
}
