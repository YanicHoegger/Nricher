using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("DependencyInjectionExtensions.Tests")]
namespace DependencyInjectionExtensions.Decorator
{
    public class InterfaceEnsurerDecorator : DispatchProxy
    {
        private object? _implementation;
        private List<MemberInfo>? _implementationExclusiveMembers;

        internal object? Decorated { get; private set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null || _implementationExclusiveMembers == null)
                return default;

            var obj = _implementationExclusiveMembers.Contains(targetMethod) ? _implementation : Decorated;
            return targetMethod.Invoke(obj, args);
        }

        public static T Create<T>([NotNull] object implementation, [NotNull] object decorated)
        {
            if (implementation == null)
                throw new ArgumentNullException(nameof(implementation));
            if (decorated == null)
                throw new ArgumentNullException(nameof(decorated));
            if (!typeof(T).IsInterface)
                throw new ArgumentException($"{typeof(T).Name} has to be an interface");

            object? proxy = Create<T, InterfaceEnsurerDecorator>();

            Debug.Assert(proxy != null, nameof(proxy) + " != null");

            ((InterfaceEnsurerDecorator)proxy).SetParameters<T>(implementation, decorated);

            return (T)proxy;
        }

        private void SetParameters<T>(object service, object decorated)
        {
            _implementation = service;
            Decorated = decorated;

            _implementationExclusiveMembers = CreateImplementationExclusiveMembers<T>(decorated);
        }

        private static List<MemberInfo> CreateImplementationExclusiveMembers<T>(object decorated)
        {
            var decoratedInterfaces = decorated.GetType().GetInterfaces().ToList();
            var serviceExclusiveInterfaces = typeof(T).GetInterfaces().Append(typeof(T)).Where(x => !decoratedInterfaces.Contains(x));

            return serviceExclusiveInterfaces.SelectMany(x => x.GetMembers()).ToList();
        }
    }
}
