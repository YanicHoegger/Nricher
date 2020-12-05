using System;

namespace DependencyInjectionExtensions.Decorator
{
    public interface IDecoratorFactory
    {
        /// <summary>
        /// Use this method to create decorator at register time
        /// </summary>
        /// <param name="toDecorate"></param>
        /// <param name="decoratingType">Interface to be decorated</param>
        /// <param name="serviceProvider"></param>
        /// <returns>Decorated as implementation of <paramref name="decoratingType"/></returns>

        //To pass IServiceProvider is the service locator anti pattern. But we using it only at register time, so it should be ok
        //TODO: Do we really need to tell which interface was implemented? We can figure that by reflection
        DecoratorResult CreateDecorated(object toDecorate, Type decoratingType, IServiceProvider serviceProvider);

        bool CanDecorate(Type decoratingType);
    }
}
