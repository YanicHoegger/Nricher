
namespace DependencyInjectionExtensions.Decorator
{
    public class SingleTypeDecoratorExtension<TService, TDecorator> : DecoratorExtension
        where TDecorator : notnull, TService
    {
        public SingleTypeDecoratorExtension() 
            : base(new SingleTypeDecoratorFactory<TService, TDecorator>())
        {
        }
    }
}
