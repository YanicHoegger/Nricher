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
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
        public InvalidMultipleConstructorDecorator(IObjectUnderTest decorated, ExtraParameter extraParameter)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1801 // Review unused parameters
            : base(decorated)
        {
        }
    }
}
