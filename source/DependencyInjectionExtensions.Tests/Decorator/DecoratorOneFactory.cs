using System;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class DecoratorOneFactory : DecoratorFactoryBase
    {
        protected override Type DecoratorType { get; } = typeof(DecoratorOne<>);
        protected override string MethodName { get; } = nameof(DecoratorOne<object>.Create);
    }
}
