using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Nricher.Decorators
{
    public class DisposableDecorator : DecoratorBase<DisposableDecorator>
    {
        private bool _isDisposed;

        protected override object? InvokeInternal(MethodInfo targetMethod, object?[]? args)
        {
            if (targetMethod == null) 
                throw new ArgumentNullException(nameof(targetMethod));

            if (targetMethod.Name.Equals(nameof(IDisposable.Dispose)))
            {
                if (_isDisposed)
                    return null;

                _isDisposed = true;
            }
            else
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(Decorated.GetType().FullName);
            }

            return targetMethod.Invoke(Decorated, args);
        }

        public static object Create([NotNull] IDisposable toDecorate)
        {
            return CreateDecorator<DisposableDecorator>(toDecorate);
        }
    }
}
