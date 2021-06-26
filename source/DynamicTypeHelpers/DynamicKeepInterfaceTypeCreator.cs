using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Nricher.DynamicTypeHelpers
{
    //Need to be static since we can not create the same type twice
    public static class DynamicKeepInterfaceTypeCreator
    {
        private static readonly Dictionary<List<Type>, Type> AlreadyCreatedInterfaceTypes = new(new ListComparer<Type>());

        public static Type Create(IEnumerable<Type> interfaces, string typeName)
        {
            var interfacesList = interfaces.OrderBy(x => x.FullName).ToList();

            var umbrellaInterface = interfacesList.FirstOrDefault(x => interfacesList.Where(y => y != x).All(y => y.IsAssignableFrom(x)));
            if (umbrellaInterface != null)
                return umbrellaInterface;

            if (AlreadyCreatedInterfaceTypes.ContainsKey(interfacesList))
                return AlreadyCreatedInterfaceTypes[interfacesList];

            var dynamicInterface = CreateDynamicInterfaceInternal(typeName, interfacesList);
            Debug.Assert(dynamicInterface != null, nameof(dynamicInterface) + " != null");

            AlreadyCreatedInterfaceTypes[interfacesList] = dynamicInterface;

            return dynamicInterface;
        }

        private static Type? CreateDynamicInterfaceInternal(string typeName, IEnumerable<Type> interfacesList)
        {
            var typeBuilder = ModuleBuilderCreator.ModuleBuilder.DefineType($"{typeName}InterfaceProxy",
                TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);

            foreach (var @interface in interfacesList)
            {
                typeBuilder.AddInterfaceImplementation(@interface);
            }

            return typeBuilder.CreateType();
        }
    }
}
