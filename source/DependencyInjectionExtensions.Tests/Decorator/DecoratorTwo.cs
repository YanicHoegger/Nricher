namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class DecoratorTwo<T> : DecoratorBase<T>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Create(T decorated)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            object proxy = Create<T, DecoratorTwo<T>>();

            ((DecoratorTwo<T>)proxy).Decorated = decorated;

            return (T)proxy;
        }
    }
}
