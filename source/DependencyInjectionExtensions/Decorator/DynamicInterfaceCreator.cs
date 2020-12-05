using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DependencyInjectionExtensions.Decorator
{
    //Need to be static since we can not create the same type twice
    //We use this to call DispatchProxyGenerator which has a static Dictionary of types
    internal static class DynamicInterfaceCreator
    {
        private static readonly ModuleBuilder ModuleBuilder = CreateModuleBuilder();
        private static readonly Dictionary<List<Type>, Type> AlreadyCreatedTypes = new Dictionary<List<Type>, Type>(new ListComparer<Type>());

        public static Type CreateDynamicInterface(IEnumerable<Type> interfaces, string typeName)
        {
            var interfacesList = interfaces.OrderBy(x => x.FullName).ToList();

            if (AlreadyCreatedTypes.ContainsKey(interfacesList))
                return AlreadyCreatedTypes[interfacesList];

            var typeBuilder = ModuleBuilder.DefineType(typeName + "Proxy",
                TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);

            foreach (var @interface in interfacesList)
            {
                typeBuilder.AddInterfaceImplementation(@interface);
            }

            var dynamicInterface = typeBuilder.CreateType();
            Debug.Assert(dynamicInterface != null, nameof(dynamicInterface) + " != null");

            AlreadyCreatedTypes[interfacesList] = dynamicInterface;

            return dynamicInterface;
        }

        private static ModuleBuilder CreateModuleBuilder()
        {
            var currentAssemblyName = typeof(DynamicInterfaceCreator).Assembly.GetName().Name;
            var assemblyName = new AssemblyName($"{currentAssemblyName}Proxies");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            return assemblyBuilder.DefineDynamicModule($"{assemblyName}ModuleBuilder");
        }
    }
}
