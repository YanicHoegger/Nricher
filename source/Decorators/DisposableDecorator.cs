using System;
using System.Diagnostics;
using System.Reflection;
using DynamicTypeHelpers;
using JetBrains.Annotations;

namespace Decorators
{
    public class DisposableDecorator : DispatchProxy
    {
        private bool _isDisposed;
        private object? _decorated;
        private NotDecoratedAttributeChecker<DisposableDecorator>? _notDecoratedAttributeChecker;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if(_decorated is null || _notDecoratedAttributeChecker is null)
                throw new InvalidOperationException($"Objects need to be created with {nameof(Create)} method");
            if (targetMethod is null)
                return null;

            if (_notDecoratedAttributeChecker.IsFiltered(targetMethod))
            {
                return targetMethod.Invoke(_decorated, args);
            }

            if (targetMethod.Name.Equals(nameof(IDisposable.Dispose)))
            {
                if (_isDisposed)
                    return null;

                _isDisposed = true;
            }
            else
            {
                if(_isDisposed)
                    throw new ObjectDisposedException(_decorated.GetType().FullName);
            }

            return targetMethod.Invoke(_decorated, args);
        }

        public static object Create([NotNull] IDisposable toDecorate)
        {
            if (toDecorate == null) 
                throw new ArgumentNullException(nameof(toDecorate));

            var @interface = DynamicTypeCreator.CreateDynamicInterface(toDecorate.GetType().GetInterfaces(), toDecorate.GetType().Name);

            var methodInfo = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create));
            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

            var proxy = (DisposableDecorator)methodInfo.MakeGenericMethod(@interface, typeof(DisposableDecorator)).Invoke(null, Array.Empty<object>())!;
            
            proxy._decorated = toDecorate;
            proxy._notDecoratedAttributeChecker = new NotDecoratedAttributeChecker<DisposableDecorator>(toDecorate);

            return proxy;
        }
    }
}
