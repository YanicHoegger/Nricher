using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Decorators
{
    public class LoggingDecorator<T> : DispatchProxy
        where T : class
    {
        private T? _decorated;
        private ILogger<T>? _logger;

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Create(T decorated, ILogger<T> logger)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            object proxy = Create<T, LoggingDecorator<T>>();

            ((LoggingDecorator<T>)proxy).SetParameters(decorated, logger);

            return (T)proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if(_decorated is null || _logger is null)
                throw new InvalidOperationException($"Objects need to be created with {nameof(Create)} method");
            if (targetMethod is null)
                return null;

            try
            {
                _logger.LogDebug($"Enter method {targetMethod.Name}");
                var result = targetMethod.Invoke(_decorated, args);
                _logger.LogDebug($"Leaving method {targetMethod.Name}");

                return result;
            }
            catch (TargetInvocationException e)
            {
                var innerException = e.InnerException;
                if (innerException != null)
                {
                    _logger.LogError($"Method {targetMethod.Name} threw exception:{Environment.NewLine}{innerException.Message}");
                    throw innerException;
                }

                _logger.LogError($"Method {targetMethod.Name} threw exception:{Environment.NewLine}{e.Message}");
                throw;
            }
        }

        private void SetParameters(T decorated, ILogger<T> logger)
        {
            _decorated = decorated;
            _logger = logger;
        }
    }
}
