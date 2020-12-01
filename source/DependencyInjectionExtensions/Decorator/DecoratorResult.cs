using System;

namespace DependencyInjectionExtensions.Decorator
{
    public class DecoratorResult
    {
        public DecoratorResult(Type decoratedType, object decorated)
        {
            DecoratedType = decoratedType;
            Decorated = decorated;
        }

        public Type DecoratedType { get; }
        public object Decorated { get; }
    }
}
