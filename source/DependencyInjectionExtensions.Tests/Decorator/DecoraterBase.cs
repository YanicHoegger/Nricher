using System.Reflection;

namespace DependencyInjectionExtensions.Tests.Decorator
{
    public class DecoratorBase<T> : DispatchProxy
    {
        protected DecoratorBase()
        {
        }

        public T Decorated { get; protected set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return targetMethod.Invoke(Decorated, args);
        }
    }
}
