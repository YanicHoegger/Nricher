using System;
using Nricher.Decorators;

namespace Decorators.Tests
{
    public class Disposable : ISomeAction, IDisposable
    {
        public void SomeAction()
        {
            //Nothing to do here
        }

        [NotDecorated]
        public void NotDecoratedAction()
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
