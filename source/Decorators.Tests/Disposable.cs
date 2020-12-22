
using System;

namespace Decorators.Tests
{
    public interface ISomeAction
    {
        void SomeAction();
    }

    public class Disposable : ISomeAction, IDisposable
    {
        public void SomeAction()
        {
            //Nothing to do here
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
