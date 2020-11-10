using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public class ServiceCollectionExtender : DispatchProxy
    {
        private IServiceCollection _decorated;
        private IEnumerable<IServiceCollectionExtension> _extensions;

        /// <summary>
        /// Do not use this constructor. Use <see cref="Create"/> method for creation. This constructor is used for activator
        /// </summary>
        // ReSharper disable once EmptyConstructor : Needed for comment
        public ServiceCollectionExtender()
        {
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            //Execute normal add method first thus the registration can be overwritten by extender
            var invoke = targetMethod.Invoke(_decorated, args);

            // ReSharper disable once InvertIf : Better readable
            if (targetMethod.Name.Equals(nameof(IServiceCollection.Add)))
            {
                foreach (var extension in _extensions)
                {
                    extension.Extend(args[0] as ServiceDescriptor, _decorated);
                }
            }

            return invoke;
        }

        public static IServiceCollection Create(IServiceCollection decorated, IEnumerable<IServiceCollectionExtension> serviceCollectionExtensions)
        {
            object proxy = Create<IServiceCollection, ServiceCollectionExtender>();

            ((ServiceCollectionExtender)proxy).SetParameters(decorated, serviceCollectionExtensions);

            return (IServiceCollection)proxy;
        }

        private void SetParameters(IServiceCollection decorated, IEnumerable<IServiceCollectionExtension> serviceCollectionExtensions)
        {
            _decorated = decorated;
            _extensions = serviceCollectionExtensions;
        }
    }
}
