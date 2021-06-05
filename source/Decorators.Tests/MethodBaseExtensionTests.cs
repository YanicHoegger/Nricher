using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Decorators.Tests
{
    public class MethodBaseExtensionTests
    {
        [Test]
        public void SameMethodOfInterfaceTest()
        {
            GivenMethodOfImplementation();
            WhenComparing();
            ThenEqual();
        }

        [Test]
        public void SameSignatureButNotImplementingTest()
        {
            GivenSameSignatureButNotOfInterfaceMethod();
            WhenComparing();
            ThenNotEqual();
        }

        [Test]
        public void OverloadMethodTest()
        {
            GivenOverloadMethod();
            WhenComparing();
            ThenNotEqual();
        }

        [Test]
        public void ExplicitImplementationTest()
        {
            GivenNotExplicitMethod();
            WhenComparing();
            ThenNotEqual();
        }

        [Test]
        public void CompareTwoImplementationsTest()
        {
            GivenMethodOfImplementation();
            WhenComparingToOtherImplementation();
            ThenEqual();
        }

        [Test]
        public void CompareSameMethodTest()
        {
            GivenMethodOfOtherImplementation();
            WhenComparingToOtherImplementation();
            ThenEqual();
        }

        private MethodInfo _givenMethod;
        private bool _result;

        private void GivenMethodOfImplementation()
        {
            _givenMethod = typeof(JustImplementation)
                .GetMethod(nameof(JustImplementation.SomeMethod));
        }

        private void GivenSameSignatureButNotOfInterfaceMethod()
        {
            _givenMethod = typeof(NotImplementingInterface)
                .GetMethod(nameof(NotImplementingInterface.SomeMethod));
        }

        private void GivenOverloadMethod()
        {
            _givenMethod = typeof(Overload)
                .GetMethods()
                .Single(x => x.GetParameters().Length == 2);
        }

        private void GivenNotExplicitMethod()
        {
            _givenMethod = typeof(ExplicitImplementation)
                .GetMethod(nameof(ExplicitImplementation.SomeMethod));
        }

        private void GivenMethodOfOtherImplementation()
        {
            _givenMethod = typeof(Overload)
                .GetMethod(nameof(Overload.SomeMethod), new[] { typeof(int) });
        }

        private void WhenComparing()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _result = typeof(IMethodInterface)
                .GetMethod(nameof(IMethodInterface.SomeMethod))
                .SameMethod(_givenMethod);
        }

        private void WhenComparingToOtherImplementation()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _result = typeof(Overload)
                .GetMethod(nameof(Overload.SomeMethod), new[] { typeof(int) })
                .SameMethod(_givenMethod);
        }

        private void ThenEqual()
        {
            Assert.That(_result, Is.True);
        }

        private void ThenNotEqual()
        {
            Assert.That(_result, Is.False);
        }
    }
}
