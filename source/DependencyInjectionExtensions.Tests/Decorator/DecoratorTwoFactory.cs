using System;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class DecoratorTwoFactory : DecoratorFactoryBase
    {
        protected override Type DecoratorType { get; } = typeof(DecoratorTwo<>);
        protected override string MethodName { get; } = nameof(DecoratorTwo<object>.Create);
    }
}
