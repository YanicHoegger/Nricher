namespace DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator
{
    public class DecoratorBase : IObjectUnderTest
    {
        protected DecoratorBase(IObjectUnderTest decorated)
        {
            Decorated = decorated;
        }

        public IObjectUnderTest Decorated { get; }
    }
}
