using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Decorators.Tests
{
    public class LoggingDecoratorTests
    {
        [Test]
        public void NormalLoggingTest()
        {
            GivenTestObject();
            WhenExecuteMethod();
            ThenLogged();
        }

        [Test]
        public void ExceptionLoggingTest()
        {
            GivenTestObject();
            WhenExecuteWithException();
            ThenExceptionLogged();
        }

        [Test]
        public async Task LoggingLeaveAfterAsyncFinished()
        {
            GivenTestObject();
            await WhenExecuteAsyncMethod().ConfigureAwait(false);
            ThenLoggedLeaveAfterFinished();
        }

        [Test]
        public async Task LoggingExceptionInAsyncMethodTest()
        {
            GivenTestObject();
            WhenExceptionInAsyncMethod();
            await WhenExecuteAsyncMethod().ConfigureAwait(false);
            ThenAsyncExceptionLogged();
        }

        private ITestObject _testObject;
        private LoggerMock<ITestObject> _loggerMock;

        private void GivenTestObject()
        {
            _loggerMock = new LoggerMock<ITestObject>();
            _testObject = LoggingDecorator<ITestObject>.Create(new TestObject(), _loggerMock);
        }

        private void WhenExecuteMethod()
        {
            _testObject.TestMethod();
        }

        private async Task WhenExecuteAsyncMethod()
        {
            await _testObject.AsyncMethod().ConfigureAwait(false);
        }

        private void WhenExecuteWithException()
        {
            try
            {
                _testObject.ThrowingMethod();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // ignored
            }
        }

        private void WhenExceptionInAsyncMethod()
        {
            _testObject.ExceptionInAsyncMethod();
        }

        private void ThenLogged()
        {
            Assert.AreEqual($"Enter method {MethodName}", _loggerMock.Logged.First());
            Assert.AreEqual($"Leaving method {MethodName}", _loggerMock.Logged.Skip(1).First());
        }

        private void ThenExceptionLogged()
        {
            CollectionAssert.Contains(_loggerMock.Logged, $"Method {nameof(ITestObject.ThrowingMethod)} threw exception:{Environment.NewLine}{TestObject.ErrorMessage}");
        }

        private void ThenLoggedLeaveAfterFinished()
        {
            Assert.AreEqual(2, _loggerMock.Logged.Count);
            CollectionAssert.Contains(_loggerMock.Logged, $"Leaving method {nameof(ITestObject.AsyncMethod)}");
        }

        private void ThenAsyncExceptionLogged()
        {
            Assert.IsTrue(_loggerMock.Logged.Last().Contains($"Method {nameof(ITestObject.AsyncMethod)} threw exception"));
        }

        private static string MethodName => nameof(ITestObject.TestMethod);
    }
}
