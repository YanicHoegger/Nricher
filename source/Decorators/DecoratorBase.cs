using System;
using System.Diagnostics;
using System.Reflection;
using DynamicTypeHelpers;
using JetBrains.Annotations;

namespace Decorators
{
    public abstract class DecoratorBase<T> : DispatchProxy
    {
        private object? _decorated;
        private NotDecoratedAttributeChecker<T>? _notDecoratedAttributeChecker;

        protected sealed override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (_decorated is null || _notDecoratedAttributeChecker is null)
                throw GetWrongCreationException();
            if (targetMethod is null)
                return null;

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_notDecoratedAttributeChecker.IsFiltered(targetMethod))
            {
                return targetMethod.Invoke(_decorated, args);
            }

            return InvokeInternal(targetMethod, args);
        }

        protected abstract object? InvokeInternal(MethodInfo targetMethod, object?[]? args);

        protected static TDecorator CreateDecorator<TDecorator>([NotNull] object toDecorate)
            where TDecorator : DecoratorBase<T>
        {
            if (toDecorate == null) 
                throw new ArgumentNullException(nameof(toDecorate));

            var @interface = DynamicKeepInterfaceTypeCreator.Create(toDecorate.GetType().GetInterfaces(), toDecorate.GetType().Name);

            var methodInfo = typeof(DispatchProxy).GetMethod(nameof(Create));
            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

            var proxy =  (TDecorator)methodInfo.MakeGenericMethod(@interface, typeof(TDecorator)).Invoke(null, Array.Empty<object>())!;

            proxy._decorated = toDecorate ?? throw new ArgumentNullException(nameof(toDecorate));
            proxy._notDecoratedAttributeChecker = new NotDecoratedAttributeChecker<T>(toDecorate);

            return proxy;
        }

        protected object Decorated => _decorated ?? throw GetWrongCreationException();

        protected InvalidOperationException GetWrongCreationException() => new("Object can not be used when created with default constructor, use static creation instead");

        //Needs double cast (first to object then to T), otherwise it wouldn't compile.
        //The CreateDecorator<TDecorator> does return a new generated Type that inherits from TDecorator and TCast
        //But the compiler doesn't know that, since this new type is made at runtime
        protected static TCast Cast<TCast>(object toCast)
        {
            return (TCast) toCast;
        }
    }
}
