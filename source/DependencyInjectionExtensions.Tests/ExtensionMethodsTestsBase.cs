using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions.Tests
{
    public class ExtensionMethodsTestsBase<T> where T : IServiceCollectionExtension, new()
    {
        protected ExtensionMethodsTestsBase()
        {
        }

        protected IServiceCollection ServiceCollection { get; private set; }

        protected void GivenServiceCollectionWithExtension()
        {
            ServiceCollection = new ServiceCollectionExtender(new ServiceCollection(), new IServiceCollectionExtension[] { new T() });
        }

        protected void GivenServiceCollectionWithoutExtension()
        {
            ServiceCollection = new ServiceCollection();
        }

        protected void WhenAddService(Expression<Action<IServiceCollection>> addServiceAction)
        {
            if (addServiceAction == null) 
                throw new ArgumentNullException(nameof(addServiceAction));

            addServiceAction.Compile()(ServiceCollection);
        }
    }
}
