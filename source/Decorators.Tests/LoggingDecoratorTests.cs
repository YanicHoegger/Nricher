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

        private void WhenExecuteWithException()
        {
            try
            {
                _loggingTestMock.Throw();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // ignored
            }
        }

        private void ThenLogged()
        {
            Assert.AreEqual($"Enter method {MethodName}", _loggerMock.Logged.First());
            Assert.AreEqual($"Leaving method {MethodName}", _loggerMock.Logged.Skip(1).First());
        }

        private void ThenExceptionLogged()
        {
            CollectionAssert.Contains(_loggerMock.Logged, $"Method {nameof(ILoggingTestMock.Throw)} threw exception:{Environment.NewLine}{LoggingTestMock.ErrorMessage}");
        }

        private static string MethodName => nameof(ILoggingTestMock.TestMethod);
    }
}
