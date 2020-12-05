using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DependencyInjectionExtensions.Decorator
{
    public class InterfaceEnsurerDecorator : DispatchProxy
    {
        private object? _service;
        private object? _decorated;
        private List<MemberInfo>? _serviceExclusiveMembers;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null || _serviceExclusiveMembers == null)
                return default;

            var obj = _serviceExclusiveMembers.Contains(targetMethod) ? _service : _decorated;
            return targetMethod.Invoke(obj, args);
        }

        public static TService Create<TService, TDecorated>(object service, TDecorated decorated)
            where TService : TDecorated
        {
            if(!typeof(TService).IsInterface)
                throw new ArgumentException($"{typeof(TService).Name} has to be an interface");
            if (!typeof(TDecorated).IsInterface)
                throw new ArgumentException($"{typeof(TDecorated).Name} has to be an interface");

            object? proxy = Create<TService, InterfaceEnsurerDecorator>();

            Debug.Assert(proxy != null, nameof(proxy) + " != null");

            ((InterfaceEnsurerDecorator)proxy).SetParameters<TService, TDecorated>(service, decorated);

            return (TService)proxy;
        }

        private void SetParameters<TService, TDecorated>(object service, TDecorated decorated)
            where TService : TDecorated
        {
            _service = service;
            _decorated = decorated;

            var decoratedInterfaces = FlattenInterfaces(typeof(TDecorated)).ToList();
            var serviceExclusiveInterfaces = FlattenInterfaces(typeof(TService)).Where(x => !decoratedInterfaces.Contains(x));
            _serviceExclusiveMembers = serviceExclusiveInterfaces.SelectMany(x => x.GetMembers()).ToList();
        }

        private static IEnumerable<Type> FlattenInterfaces(Type t)
        {
            return t.GetInterfaces().Append(t);
        }
    }
}
