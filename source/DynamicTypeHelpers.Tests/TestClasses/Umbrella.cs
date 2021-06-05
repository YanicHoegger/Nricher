namespace DynamicTypeHelpers.Tests.TestClasses
{
#pragma warning disable CA1040 // Avoid empty interfaces : Is for a test
    public interface ISomething
    {
    }

    public interface IAnOtherThing
    {
    }

    public interface IUmbrella : ISomething, IAnOtherThing
#pragma warning restore CA1040 // Avoid empty interfaces
    {
    }

    public class Umbrella : IUmbrella
    {
    }
}
