namespace DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator
{
    public class TestDecorator : DecoratorBase
    {
        public TestDecorator(IObjectUnderTest decorated) 
            : base(decorated)
        {
        }
    }
}
