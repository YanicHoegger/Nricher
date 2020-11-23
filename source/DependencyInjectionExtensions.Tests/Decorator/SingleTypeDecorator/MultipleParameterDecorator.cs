using System;

namespace DependencyInjectionExtensions.Tests.Decorator.SingleTypeDecorator
{
    public class MultipleParameterDecorator : DecoratorBase
    {
        public MultipleParameterDecorator(IObjectUnderTest decorated, ExtraParameter extraParameter) 
            : base(decorated)
        {
            if (extraParameter == null) 
                throw new ArgumentNullException(nameof(extraParameter));
        }
    }

    public class ExtraParameter
    {
    }
}
