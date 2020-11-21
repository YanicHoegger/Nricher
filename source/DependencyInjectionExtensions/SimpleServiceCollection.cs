using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionExtensions
{
    public class SimpleServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
    }
}
