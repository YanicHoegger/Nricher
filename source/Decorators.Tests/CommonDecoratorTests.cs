using System;
using NUnit.Framework;

// ReSharper disable SuspiciousTypeConversion.Global
namespace Decorators.Tests
{
    public class CommonDecoratorTests
    {
        [Test]
        public void NoDecorationTest()
        {
            GivenDecorated();
            WhenExecuteMethod();
            ThenNoExceptions();
        }

        [Test]
        public void EnteringFiresTest()
        {
            GivenDecorated();
            WhenAddEntering();
            WhenExecuteMethod();
            ThenFired();
        }

        [Test]
        public void LeavingFiresTest()
        {
            GivenDecorated();
            WhenAddLeaving();
            WhenExecuteMethod();
            ThenFired();
        }

        [Test]
        public void ExceptionFiresTest()
        {
            GivenDecorated();
            WhenAddException();
            WhenException();
            ThenFired();
        }

        private ITestObject _testObject;
        private bool _hasFired;

        private void GivenDecorated()
        {
            _testObject = CommonDecorator<ITestObject>.Create(new TestObject());
        }

        private void WhenExecuteMethod()
        {
            _testObject.TestMethod();
        }

        private void WhenException()
        {
            try
            {
                _testObject.ThrowingMethod();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
            }
        }

        private void WhenAddEntering()
        {
            ((CommonDecorator<ITestObject>) _testObject).Enter = () => _hasFired = true;
        }

        private void WhenAddLeaving()
        {
            ((CommonDecorator<ITestObject>)_testObject).Leave = result => _hasFired = true;
        }

        private void WhenAddException()
        {
            ((CommonDecorator<ITestObject>)_testObject).Exception = exception => _hasFired = true;
        }

        private static void ThenNoExceptions()
        {
            //Nothing to do here
        }

        private void ThenFired()
        {
            Assert.IsTrue(_hasFired);
        }
    }
}
