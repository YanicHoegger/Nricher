
namespace DependencyInjectionExtensions.Tests.Decorator.DynamicTypeCreator
{
    public class MultipleConstructorClass
    {
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
        // ReSharper disable UnusedParameter.Local
        public MultipleConstructorClass(string something)
        {
        }

        public MultipleConstructorClass(IObjectUnderTest anOtherThing)
            // ReSharper restore UnusedParameter.Local
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore IDE0060 // Remove unused parameter
        {

        }
    }
}
