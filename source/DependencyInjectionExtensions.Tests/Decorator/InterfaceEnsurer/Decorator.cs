
namespace DependencyInjectionExtensions.Tests.Decorator.InterfaceEnsurer
{
    public class Decorator : IBaseInterface
    {
        public const string DecoratedReturnValue = "Decorated";
        public string BaseCall => DecoratedReturnValue;
    }
}
