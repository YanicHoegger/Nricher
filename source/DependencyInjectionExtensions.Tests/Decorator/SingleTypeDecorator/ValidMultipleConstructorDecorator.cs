namespace DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator
{
    public class ValidMultipleConstructorDecorator : DecoratorBase
    {
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        // ReSharper disable UnusedMember.Global
        private ValidMultipleConstructorDecorator(IObjectUnderTest decorated, ExtraParameter extraParameter)
            : base(decorated)
        {
        }

        protected ValidMultipleConstructorDecorator(IObjectUnderTest decorated, 
            ExtraParameter extraParameter, 
            ExtraParameter otherExtraParameter) 
            : base(decorated)
        {
        }

        internal ValidMultipleConstructorDecorator(IObjectUnderTest decorated,
            ExtraParameter extraParameter,
            ExtraParameter otherExtraParameter,
            ExtraParameter anOtherExtraParameter)
            : base(decorated)
        {
        }


        public ValidMultipleConstructorDecorator(IObjectUnderTest decorated) 
            : base(decorated)
        {
        }
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedMember.Global
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
