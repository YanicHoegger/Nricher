using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    // ReSharper disable once UnusedTypeParameter : Is used for DI
    //TODO: Make internal
    public class DecoratorContainer<TDecorated, TDecorator>
    {
        public DecoratorContainer(TDecorated decorated)
        {
            Decorated = decorated;
        }

        public TDecorated Decorated { get; }

        //TODO:
        public ServiceDescriptor Original { get; }
    }
}
