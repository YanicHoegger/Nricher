using DependencyInjectionExtensions.Decorator;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator.InterfaceEnsurer
{
    public class InterfaceEnsurerTests
    {
        [Test]
        public void DecoratedMethodTest()
        {
            GivenDecoratorAndOriginal();
            WhenEnsureInterface();
            DecoratorGetsCalledOnDecoratedInterface();
        }

        [Test]
        public void NotDecoratedMethodTest()
        {
            GivenDecoratorAndOriginal();
            WhenEnsureInterface();
            OriginalGetsCalledOnOriginalInterface();
        }

        private ICombined _original;
        private IBaseInterface _decorated;

        private ICombined _ensured;

        private void GivenDecoratorAndOriginal()
        {
            _original = new Combined();
            _decorated = new Decorator();
        }

        private void WhenEnsureInterface()
        {
            _ensured = InterfaceEnsurerDecorator.Create(_original, _decorated);
        }

        private void DecoratorGetsCalledOnDecoratedInterface()
        {
            Assert.AreEqual(Decorator.DecoratedReturnValue, _ensured.BaseCall);
        }

        private void OriginalGetsCalledOnOriginalInterface()
        {
            Assert.AreEqual(Combined.CombinedReturnValue, _ensured.OtherCall);
        }
    }
}
