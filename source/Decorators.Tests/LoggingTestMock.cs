using System;

namespace Decorators.Tests
{
    public interface ILoggingTestMock
    {
        void TestMethod();
        void Throw();
    }

    public class LoggingTestMock : ILoggingTestMock
    {
        public void TestMethod()
        {
            //Nothing to do here
        }

        public static string ErrorMessage { get; } = nameof(ErrorMessage);

        public void Throw()
        {
            throw new Exception(ErrorMessage);
        }
    }
}
