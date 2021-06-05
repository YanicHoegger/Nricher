using System;
using System.Collections.Generic;
using DynamicTypeHelpers.Tests.TestClasses;
using NUnit.Framework;

namespace DynamicTypeHelpers.Tests
{
    public class DynamicKeepInterfaceTypeCreatorTests
    {
        [Test]
        public void CreatedTypeImplementsAllInterfaces()
        {
            GivenInterfaces();
            WhenCreateDynamicInterface();
            ThenAllInterfacesImplemented();
        }

        [Test]
        public void NotCreatingTypeWhenUmbrellaInterface()
        {
            GivenUmbrellaInterfaces();
            WhenCreateDynamicInterface();
            ThenResultingTypeIsUmbrellaInterface();
        }

        //TODO: Create same type twice test

        private IEnumerable<Type> _interfaces;
        private Type _dynamicInterface;

        private void GivenInterfaces()
        {
            _interfaces = new[] { typeof(ISomething), typeof(IAnOtherThing) };
        }

        private void GivenUmbrellaInterfaces()
        {
            _interfaces = typeof(Umbrella).GetInterfaces();
        }

        private void WhenCreateDynamicInterface()
        {
            _dynamicInterface = DynamicKeepInterfaceTypeCreator.Create(_interfaces, nameof(Umbrella));
        }

        private void ThenAllInterfacesImplemented()
        {
            foreach (var @interface in _interfaces)
            {
                Assert.IsTrue(@interface.IsAssignableFrom(_dynamicInterface));
            }
        }

        private void ThenResultingTypeIsUmbrellaInterface()
        {
            Assert.AreEqual(typeof(IUmbrella), _dynamicInterface);
        }
    }
}
