using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public class ServiceCollectionExtender : IServiceCollection
    {
        private readonly IServiceCollection _decorated;

        public ServiceCollectionExtender(IServiceCollection decorated, IEnumerable<IServiceCollectionExtension> extensions)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            Extensions = extensions ?? throw new ArgumentNullException(nameof(extensions));
        }

        public IEnumerable<IServiceCollectionExtension> Extensions { get; }

        public void Add(ServiceDescriptor item)
        {
            Add(item, Extensions);
        }

        public void Add(ServiceDescriptor item, IEnumerable<IServiceCollectionExtension> extensions)
        {
            if (extensions == null) 
                throw new ArgumentNullException(nameof(extensions));

            //Execute normal add method first thus the registration can be overwritten by extender
            _decorated.Add(item);

            foreach (var extension in extensions)
            {
                extension.Extend(item, _decorated);
            }
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator() => _decorated.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear() => _decorated.Clear();

        public bool Contains(ServiceDescriptor item) => _decorated.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _decorated.CopyTo(array, arrayIndex);

        public bool Remove(ServiceDescriptor item) => _decorated.Remove(item);

        public int Count => _decorated.Count;
        public bool IsReadOnly => _decorated.IsReadOnly;
        public int IndexOf(ServiceDescriptor item) => _decorated.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item) => _decorated.Insert(index, item);

        public void RemoveAt(int index) => _decorated.RemoveAt(index);

        public ServiceDescriptor this[int index]
        {
            get => _decorated[index];
            set => _decorated[index] = value;
        }
    }
}
