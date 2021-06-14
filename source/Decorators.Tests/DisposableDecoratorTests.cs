using System;
using Nricher.Decorators;
using NUnit.Framework;

namespace Decorators.Tests
{
    public class DisposableDecoratorTests
    {
        [Test]
        public void ThrowWhenDisposedTest()
        {
            GivenDecorated();
            WhenDispose();
            ThenThrowsWhenCallingMethod();
        }

        [Test]
        public void DoExecuteWhenNotDisposedTest()
        {
            GivenDecorated();
            WhenNotDispose();
            ThenDoesNotThrowWhenCallingMethod();
        }

        [Test]
        public void DoExecuteWhenNotDecoratedTest()
        {
            GivenDecorated();
            WhenDispose();
            ThenDoesNotThrowWhenCallingNotDecoratedMethod();
        }

        private object _decorated;

        private void GivenDecorated()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope : New scope is in decorated
            _decorated = DisposableDecorator.Create(new Disposable());
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        private void WhenDispose()
        {
            ((IDisposable)_decorated).Dispose();
        }

        private static void WhenNotDispose()
        {
            //Nothing to do here
        }

        private void ThenThrowsWhenCallingMethod()
        {
            Assert.Throws<ObjectDisposedException>(() => ((ISomeAction) _decorated).SomeAction());
        }

        private void ThenDoesNotThrowWhenCallingMethod()
        {
            ((ISomeAction) _decorated).SomeAction();
        }

        private void ThenDoesNotThrowWhenCallingNotDecoratedMethod()
        {
            ((ISomeAction)_decorated).NotDecoratedAction();
        }
    }
}
