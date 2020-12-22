using System;
using System.Threading.Tasks;

namespace Decorators.Tests
{
    public interface ILoggingTestMock
    {
        void TestMethod();
        void ThrowingMethod();

        Task AsyncMethod();
        void FinishAsyncMethod();
        void ExceptionInAsyncMethod();
    }

    public class LoggingTestMock : ILoggingTestMock
    {
        private bool _asyncFinished;
        private bool _throwException;

        public void TestMethod()
        {
            //Nothing to do here
        }

        public static string ErrorMessage { get; } = nameof(ErrorMessage);

        public void ThrowingMethod()
        {
            throw new Exception(ErrorMessage);
        }

        public Task AsyncMethod()
        {
            return Task.Run(AsyncMethodInternal);
        }

        public void FinishAsyncMethod()
        {
            _asyncFinished = true;
        }

        public void ExceptionInAsyncMethod()
        {
            _throwException = true;
            _asyncFinished = true;
        }

        private void AsyncMethodInternal()
        {
            _asyncFinished = false;

            while (!_asyncFinished)
            {
            }

            if (_throwException)
            {
                throw new Exception(ErrorMessage);
            }
        }
    }
}
