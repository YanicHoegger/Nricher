using System;
using System.Reflection;

namespace Decorators
{
    public class CommonDecorator<T> : DispatchProxy
        where T : class
    {
        public T? Decorated { get; private set; }

        public Action? Enter { get; set; }
        public Action<object?>? Leave { get; set; }
        public Action<Exception>? Exception { get; set; }

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Create(T decorated)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            object proxy = Create<T, CommonDecorator<T>>();

            ((CommonDecorator<T>) proxy).Decorated = decorated;

            return (T)proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (Decorated is null)
                throw new InvalidOperationException($"Objects need to be created with {nameof(Create)} method");
            if (targetMethod is null)
                return null;

            try
            {
                Enter?.Invoke();

                var result = targetMethod.Invoke(Decorated, args);

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
