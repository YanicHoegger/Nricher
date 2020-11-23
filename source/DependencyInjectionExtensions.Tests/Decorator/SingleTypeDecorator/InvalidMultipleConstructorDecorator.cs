// ReSharper disable UnusedMember.Global
namespace DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator
{
    public class InvalidMultipleConstructorDecorator : DecoratorBase
    {
        public InvalidMultipleConstructorDecorator(IObjectUnderTest decorated) 
            : base(decorated)
        {
        }

        // ReSharper disable once UnusedParameter.Local
#pragma warning disable IDE0060 // Remove unused parameter
        public InvalidMultipleConstructorDecorator(IObjectUnderTest decorated, ExtraParameter extraParameter)
#pragma warning restore IDE0060 // Remove unused parameter
            : base(decorated)
        {
        }
    }
}
