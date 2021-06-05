using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Nricher.Decorators
{
    //TODO: Make unit tests
    public class CommonDecorator<T> : DecoratorBase<CommonDecorator<T>>
    {
        public new T? Decorated => (T)base.Decorated;

        public Action? Enter { get; set; }
        public Action<object?>? Leave { get; set; }
        public Action<Exception>? Exception { get; set; }

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Create([NotNull] T toDecorate)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            if (toDecorate == null) 
                throw new ArgumentNullException(nameof(toDecorate));

            
            return Cast<T>(CreateDecorator<CommonDecorator<T>>(toDecorate));
        }

        protected override object? InvokeInternal(MethodInfo targetMethod, object?[]? args)
        {
            try
            {
                Enter?.Invoke();

#pragma warning disable CA1062 // Validate arguments of public methods : Is checked in base method
                var result = targetMethod.Invoke(Decorated, args);
#pragma warning restore CA1062 // Validate arguments of public methods

                Leave?.Invoke(result);

                return result;
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
                throw;
            }
        }
    }
}
