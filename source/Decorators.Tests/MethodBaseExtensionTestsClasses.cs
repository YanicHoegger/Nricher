// ReSharper disable UnusedMember.Global
namespace Decorators.Tests
{
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
    public interface IMethodInterface
    {
        void SomeMethod(int i);
    }

    public class JustImplementation : IMethodInterface
    {
        public void SomeMethod(int i)
        {
        }
    }

    public class NotImplementingInterface
    {

        public void SomeMethod(int i)
        {
        }
    }

    public class Overload : IMethodInterface
    {
        public void SomeMethod(int i)
        {
        }

        public void SomeMethod(int i, object anOtherParameter)

        {
        }
    }

    public class ExplicitImplementation : IMethodInterface
    {
        void IMethodInterface.SomeMethod(int i)
        {
        }

        public void SomeMethod(int i)
        {
        }
    }
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore IDE0060 // Remove unused parameter
}
