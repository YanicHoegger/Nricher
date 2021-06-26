
namespace DependencyInjectionExtensions.Tests.Decorator.InterfaceEnsurer
{
    public class Combined : ICombined
    {
        public const string CombinedReturnValue = "Combined";

        public string BaseCall => CombinedReturnValue;
        public string OtherCall => CombinedReturnValue;
    }
}
