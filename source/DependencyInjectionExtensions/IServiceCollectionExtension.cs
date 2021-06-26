using Microsoft.Extensions.DependencyInjection;

namespace Nricher.DependencyInjectionExtensions
{
    public interface IServiceCollectionExtension
    {
        void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection);
    }
}
