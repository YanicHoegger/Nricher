
namespace DependencyInjectionExtensions.Tests.Decorator.InterfaceEnsurer
{
    public class DecoratorImplementation : IBaseInterface
    {
        public const string DecoratedReturnValue = "Decorated";
        public string BaseCall => DecoratedReturnValue;
    }
}
