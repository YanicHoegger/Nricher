using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeHelpers
{
    public static class ModuleBuilderCreator
    {
        public static readonly ModuleBuilder ModuleBuilder = CreateModuleBuilder();

        private static ModuleBuilder CreateModuleBuilder()
        {
            var currentAssemblyName = typeof(DynamicKeepInterfaceTypeCreator).Assembly.GetName().Name;
            var assemblyName = new AssemblyName($"{currentAssemblyName}Proxies");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            return assemblyBuilder.DefineDynamicModule($"{assemblyName}ModuleBuilder");
        }

    }
}
