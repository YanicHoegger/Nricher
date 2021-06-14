
namespace DynamicTypeHelpers.Tests.TestClasses
{
    public class MultipleConstructorClass
    {
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
        // ReSharper disable UnusedParameter.Local
        // ReSharper disable UnusedMember.Global
        public MultipleConstructorClass(string something)
        {
        }

        public MultipleConstructorClass(IObjectUnderTest anOtherThing)
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Global
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore IDE0060 // Remove unused parameter
        {

        }
    }
}
