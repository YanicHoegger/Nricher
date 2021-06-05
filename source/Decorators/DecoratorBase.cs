using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nricher.DynamicTypeHelpers;

namespace Nricher.Decorators
{
    public abstract class DecoratorBase : DispatchProxy
    {
        public abstract object Decorated { get; }
    }

    public abstract class DecoratorBase<T> : DecoratorBase
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
            if (_notDecoratedAttributeChecker.IsFiltered(targetMethod) || IgnoringMethods.Any(x => x.SameMethod(targetMethod)))
            {
                return targetMethod.Invoke(_decorated, args);
            }

            return InvokeInternal(targetMethod, args);
        }

        public List<MethodInfo> IgnoringMethods { get; } = new();

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
            proxy._notDecoratedAttributeChecker = new NotDecoratedAttributeChecker<T>(proxy.GetCascadedDecorated());

            return proxy;
        }

        public override object Decorated => _decorated ?? throw GetWrongCreationException();

        protected InvalidOperationException GetWrongCreationException() => new("Object can not be used when created with default constructor, use static creation instead");

        //Needs double cast (first to object then to T), otherwise it wouldn't compile.
        //The CreateDecorator<TDecorator> does return a new generated Type that inherits from TDecorator and TCast
        //But the compiler doesn't know that, since this new type is made at runtime
        protected static TCast Cast<TCast>(object toCast)
        {
            return (TCast) toCast;
        }

        private object GetCascadedDecorated()
        {
            var cascaded = Decorated;
            for (; cascaded is DecoratorBase decorator; cascaded = decorator.Decorated)
            {
            }

            return cascaded;
        }
    }
}
