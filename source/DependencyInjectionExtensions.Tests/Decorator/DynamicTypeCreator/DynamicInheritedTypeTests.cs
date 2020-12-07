using System;
using System.Reflection;
using NUnit.Framework;

namespace DependencyInjectionExtensions.Tests.Decorator.DynamicTypeCreator
{
    public class DynamicInheritedTypeTests
    {
        [Test]
        public void InterfaceTypeThrowsTest()
        {
            GivenInterfaceType();
            WhenCreateThenThrow();
        }

        [Test]
        public void AbstractTypeThrowsTest()
        {
            GivenAbstractType();
            WhenCreateThenThrow();
        }

        [Test]
        public void SealedTypeThrowsTest()
        {
            GivenSealedType();
            WhenCreateThenThrow();
        }

        [Test]
        public void MultipleConstructorThrowsTest()
        {
            GivenMultipleConstructorType();
            WhenCreateThenThrow();
        }

        [Test]
        public void PrivateConstructorThrowsTest()
        {
            GivenPrivateConstructorType();
            WhenCreateThenThrow();
        }

        [Test]
        public void DefaultConstructorTypeTest()
        {
            GivenEmptyConstructorType();
            WhenCreate();
            ThenTypeCreated();
        }

        [Test]
        public void MultipleParametersConstructorTest()
        {
            GivenMultipleParametersConstructorType();
            WhenCreate();
            ThenTypeCreated();
            ThenMultipleParameterConstructorCanBeActivated();
        }

        private Type _originalType;
        private Type _createdType;

        private void GivenInterfaceType()
        {
            _originalType = typeof(IDisposable);
        }

        private void GivenAbstractType()
        {
            _originalType = typeof(DispatchProxy);
        }

        private void GivenSealedType()
        {
            _originalType = typeof(SealedType);
        }

        private void GivenMultipleConstructorType()
        {
            _originalType = typeof(MultipleConstructorClass);
        }

        private void GivenPrivateConstructorType()
        {
            _originalType = typeof(PrivateConstructorTest);
        }

        private void GivenEmptyConstructorType()
        {
            _originalType = typeof(ObjectUnderTest);
        }

        private void GivenMultipleParametersConstructorType()
        {
            _originalType = typeof(MultipleParameterConstructor);
        }

        private void WhenCreate()
        {
            _createdType = DependencyInjectionExtensions.Decorator.DynamicTypeCreator.CreateInheritedType(_originalType);
        }

        private void WhenCreateThenThrow()
        {
            Assert.Throws<ArgumentException>(WhenCreate);
        }

        private void ThenTypeCreated()
        {
            Assert.IsNotNull(_createdType);
            Assert.AreNotEqual(_originalType, _createdType);
        }

        private void ThenMultipleParameterConstructorCanBeActivated()
        {
            var activated = (MultipleParameterConstructor)Activator.CreateInstance(_createdType, 2, 3, "Hello", new ObjectUnderTest());

            Assert.IsNotNull(activated);
            Assert.AreEqual(2, activated.Value1);
            Assert.AreEqual(3, activated.Value2);
            Assert.AreEqual("Hello", activated.OtherValue);
            Assert.IsNotNull(activated.ReferenceVale);
        }
    }
}
