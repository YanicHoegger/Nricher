namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class DecoratorOne<T> : DecoratorBase<T>
    {
        public static T Create(T decorated)
        {
            object proxy = Create<T, DecoratorOne<T>>();

            ((DecoratorOne<T>)proxy).Decorated = decorated;

            return (T)proxy;
        }
    }
}
