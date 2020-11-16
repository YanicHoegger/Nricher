using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public interface IServiceCollectionExtension
    {
        void Extend(ServiceDescriptor serviceDescriptor, IServiceCollection serviceCollection);
    }
}
