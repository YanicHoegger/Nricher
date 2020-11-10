namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class DecoratorTwo<T> : DecoratorBase<T>
    {
        public static T Create(T decorated)
        {
            object proxy = Create<T, DecoratorTwo<T>>();

            ((DecoratorTwo<T>)proxy).Decorated = decorated;

            return (T)proxy;
        }
    }
}
