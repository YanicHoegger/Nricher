using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Decorators
{
    public class LoggingDecorator<T> : DispatchProxy
        where T : class
    {
        private T? _decorated;
        private ILogger<T>? _logger;
        private TaskScheduler? _loggingScheduler;

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Create(T decorated, ILogger<T> logger)
        {
            return Create(decorated, logger, TaskScheduler.Default);
        }

        public static T Create(T decorated, ILogger<T> logger, TaskScheduler taskScheduler)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            object proxy = Create<T, LoggingDecorator<T>>();

            ((LoggingDecorator<T>)proxy).SetParameters(decorated, logger, taskScheduler);

            return (T)proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if(_decorated is null || _logger is null || _loggingScheduler is null)
                throw new InvalidOperationException($"Objects need to be created with {nameof(Create)} method");
            if (targetMethod is null)
                return null;

            try
            {
                _logger.LogDebug($"Enter method {targetMethod.Name}");

                var result = targetMethod.Invoke(_decorated, args);

                if (result is Task asyncResult)
                {
                    _ = asyncResult.ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            LogException(targetMethod, task.Exception);
                        }
                        else
                        {
                            LogLeaving(targetMethod);
                        }
                    }, _loggingScheduler);
                }
                else
                {
                    LogLeaving(targetMethod);
                }


                return result;
            }
            catch (TargetInvocationException e)
            {
                var innerException = e.InnerException;
                if (innerException != null)
                {
                    LogException(targetMethod, innerException);
                    throw innerException;
                }

                LogException(targetMethod, e);
                throw;
            }
        }

        private void LogException(MemberInfo targetMethod, Exception exception)
        {
            _logger.LogError($"Method {targetMethod.Name} threw exception:{Environment.NewLine}{exception.Message}");
        }

        private void LogLeaving(MemberInfo targetMethod)
        {
            _logger.LogDebug($"Leaving method {targetMethod.Name}");
        }

        private void SetParameters(T decorated, ILogger<T> logger, TaskScheduler taskScheduler)
        {
            _decorated = decorated;
            _logger = logger;
            _loggingScheduler = taskScheduler;
        }
    }
}
