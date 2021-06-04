using System;
using System.Threading.Tasks;

namespace Decorators.Tests
{
    public interface ITestObject
    {
        void TestMethod();
        void ThrowingMethod();

        Task AsyncMethod();
        void ExceptionInAsyncMethod();
    }

    public class TestObject : ITestObject
    {
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

        public void ExceptionInAsyncMethod()
        {
            _throwException = true;
        }

        private void AsyncMethodInternal()
        {
            if (_throwException)
            {
                throw new Exception(ErrorMessage);
            }
        }
    }
}
