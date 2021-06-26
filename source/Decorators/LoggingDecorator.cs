using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nricher.Decorators
{
    public class LoggingDecorator<T> : DecoratorBase<LoggingDecorator<T>>
        where T : class
    {
        private ILogger<T>? _logger;
        private TaskScheduler? _loggingScheduler;

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Create(T toDecorate, ILogger<T> logger)
        {
            return Create(toDecorate, logger, TaskScheduler.Default);
        }

        public static T Create(T toDecorate, ILogger<T> logger, TaskScheduler taskScheduler)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            var proxy = CreateDecorator<LoggingDecorator<T>>(toDecorate);

            proxy.SetParameters(logger, taskScheduler);

            return Cast<T>(proxy);
        }

        protected override object? InvokeInternal(MethodInfo targetMethod, object?[]? args)
        {
            if (_logger is null || _loggingScheduler is null)
                throw GetWrongCreationException();

            try
            {
#pragma warning disable CA1062 // Validate arguments of public methods : Is checked in base class
                _logger.LogDebug($"Enter method {targetMethod.Name}");
#pragma warning restore CA1062 // Validate arguments of public methods

                var result = targetMethod.Invoke(Decorated, args);

                if (result is Task asyncResult)
                {
                    result = asyncResult.ContinueWith(task =>
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

        private void SetParameters(ILogger<T> logger, TaskScheduler taskScheduler)
        {
            _logger = logger;
            _loggingScheduler = taskScheduler;
        }
    }
}
