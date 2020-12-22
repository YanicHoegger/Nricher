using System;
using System.Linq;
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
        public void LoggingLeaveAfterAsyncFinished()
        {
            GivenTestObject();
            WhenExecuteAsyncMethod();
            ThenLoggedLeaveAfterFinished();
        }

        [Test]
        public void LoggingExceptionInAsyncMethodTest()
        {
            GivenTestObject();
            WhenExecuteAsyncMethod();
            WhenExceptionInAsyncMethod();
            ThenAsyncExceptionLogged();
        }

        private ILoggingTestMock _loggingTestMock;
        private LoggerMock<ILoggingTestMock> _loggerMock;

        private void GivenTestObject()
        {
            _loggerMock = new LoggerMock<ILoggingTestMock>();
            _loggingTestMock = LoggingDecorator<ILoggingTestMock>.Create(new LoggingTestMock(), _loggerMock);
        }

        private void WhenExecuteMethod()
        {
            _loggingTestMock.TestMethod();
        }

        private void WhenExecuteAsyncMethod()
        {
            _loggingTestMock.AsyncMethod();
        }

        private void WhenExecuteWithException()
        {
            try
            {
                _loggingTestMock.ThrowingMethod();
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
            _loggingTestMock.ExceptionInAsyncMethod();
            while (_loggerMock.Logged.Count() < 4)
            {
            }
        }

        private void ThenLogged()
        {
            Assert.AreEqual($"Enter method {MethodName}", _loggerMock.Logged.First());
            Assert.AreEqual($"Leaving method {MethodName}", _loggerMock.Logged.Skip(1).First());
        }

        private void ThenExceptionLogged()
        {
            CollectionAssert.Contains(_loggerMock.Logged, $"Method {nameof(ILoggingTestMock.ThrowingMethod)} threw exception:{Environment.NewLine}{LoggingTestMock.ErrorMessage}");
        }

        private void ThenLoggedLeaveAfterFinished()
        {
            Assert.AreEqual(1, _loggerMock.Logged.Count());

            _loggingTestMock.FinishAsyncMethod();
            while (_loggerMock.Logged.Count() < 4)
            {
            }

            CollectionAssert.Contains(_loggerMock.Logged, $"Leaving method {nameof(ILoggingTestMock.AsyncMethod)}");
        }


        private void ThenAsyncExceptionLogged()
        {
            Assert.IsTrue(_loggerMock.Logged.Last().Contains($"Method {nameof(ILoggingTestMock.AsyncMethod)} threw exception"));
        }


        private static string MethodName => nameof(ILoggingTestMock.TestMethod);
    }
}
