using System;
using System.Diagnostics;
using System.Reflection;
using DynamicTypeHelpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace Decorators
{
    public class HostedServiceDecorator : DispatchProxy
    {
        private bool _isStarted;
        private object? _decorated;
        private NotDecoratedAttributeChecker<HostedServiceDecorator>? _notDecoratedAttributeChecker;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (_decorated is null || _notDecoratedAttributeChecker is null)
                throw new InvalidOperationException($"Objects need to be created with {nameof(Create)} method");
            if (targetMethod is null)
                return null;

            if (_notDecoratedAttributeChecker.IsFiltered(targetMethod))
            {
                return targetMethod.Invoke(_decorated, args);
            }

            if (targetMethod.Name.Equals(nameof(IHostedService.StartAsync)))
            {
                _isStarted = true;
            }
            else if (targetMethod.Name.Equals(nameof(IHostedService.StopAsync)))
            {
                _isStarted = false;
            }
            else
            {
                if (!_isStarted)
                    throw new InvalidOperationException($"{_decorated.GetType().Name} needs to be started");
            }

            return targetMethod.Invoke(_decorated, args);
        }

        public static object Create([NotNull] IHostedService hostedService)
        {
            if (hostedService == null)
                throw new ArgumentNullException(nameof(hostedService));

            var @interface = DynamicTypeCreator.CreateDynamicInterface(hostedService.GetType().GetInterfaces(), hostedService.GetType().Name);

            var methodInfo = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create));
            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");

            var proxy = (HostedServiceDecorator)methodInfo.MakeGenericMethod(@interface, typeof(HostedServiceDecorator)).Invoke(null, Array.Empty<object>())!;

            proxy._decorated = hostedService;
            proxy._notDecoratedAttributeChecker = new NotDecoratedAttributeChecker<HostedServiceDecorator>(hostedService);

            return proxy;
        }
    }
}
