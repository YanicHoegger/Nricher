using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public static class ServiceCollectionHelper
    {
        public static void ReplaceServiceDescriptor(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection)
        {
            var toReplace = serviceCollection.Last(x => x.ServiceType == serviceDescriptor.ServiceType);
            var index = serviceCollection.IndexOf(toReplace);
            serviceCollection[index] = serviceDescriptor;
        }
    }
}
