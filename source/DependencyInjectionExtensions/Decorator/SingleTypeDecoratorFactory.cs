using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.Decorator
{
    /// <summary>
    /// Use this extension for decorating implementations of a given service type
    /// </summary>
    /// <typeparam name="TService">Service type</typeparam>
    /// <typeparam name="TDecorator">Decorator</typeparam>
    public class SingleTypeDecoratorFactory<TService, TDecorator> : IDecoratorFactory
        where TDecorator : notnull, TService
    {
        private readonly ConstructorInfo _constructorInfo;

        public SingleTypeDecoratorFactory()
        {
            var constructorInfos = typeof(TDecorator).GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            if (constructorInfos.Length != 1)
                throw new InvalidOperationException($"Decorators must have one public constructor. {typeof(TDecorator).Name} has {constructorInfos.Length}");

            _constructorInfo = constructorInfos.Single();
        }

        public object CreateDecorated(object toDecorate, Type decoratingType, IServiceProvider serviceProvider)
        {
            CheckInput(toDecorate, decoratingType, serviceProvider);

            object CreateParameter(ParameterInfo parameterInfo)
            {
                var parameterType = parameterInfo.ParameterType;
                return parameterType == typeof(TService) ? toDecorate : serviceProvider.GetRequiredService(parameterType);
            }

            var parameters = _constructorInfo
                .GetParameters()
                .Select(CreateParameter)
                .ToArray();

            var decorated = Activator.CreateInstance(typeof(TDecorator), parameters);

            if (decorated == null)
                throw new InvalidOperationException($"Could not create instance of {typeof(TDecorator).Name}");

            return decorated;
        }

        public bool CanDecorate(Type decoratingType)
        {
            return typeof(TService) == decoratingType;
        }

        [AssertionMethod]
        private static void CheckInput(object toDecorate, Type decoratingType, IServiceProvider serviceProvider)
        {
            if (toDecorate == null)
                throw new ArgumentNullException(nameof(toDecorate));
            if (decoratingType == null)
                throw new ArgumentNullException(nameof(decoratingType));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (decoratingType != typeof(TService))
                throw new ArgumentException($"{nameof(decoratingType)} has to be {typeof(TService).Name}");
            if (!(toDecorate is TService))
                throw new ArgumentException($"{nameof(toDecorate)} has to be assignable to {typeof(TService).Name}");
        }
    }
}
