using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Nricher.Decorators
{
    public static class MethodBaseExtensions
    {
        public static bool SameMethod([NotNull] this MethodInfo a, [NotNull] MethodInfo b)
        {
            if (a == null) 
                throw new ArgumentNullException(nameof(a));
            if (b == null) 
                throw new ArgumentNullException(nameof(b));

            return GetDeclaringMethodInfo(a) == GetDeclaringMethodInfo(b);
        }

        private static MethodInfo GetDeclaringMethodInfo(MethodInfo methodInfo)
        {
            return GetInterfaceDeclarationsForMethod(methodInfo).SingleOrDefault() ?? methodInfo;
        }

        private static IEnumerable<MethodInfo> GetInterfaceDeclarationsForMethod(this MethodInfo methodInfo)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return methodInfo.ReflectedType
#pragma warning restore CS8604 // Possible null reference argument.
                .GetAllInterfaceMaps()
                .SelectMany(map => Enumerable.Range(0, map.TargetMethods.Length)
                    .Where(n => map.TargetMethods[n] == methodInfo)
                    .Select(n => map.InterfaceMethods[n]));
        }

        private static IEnumerable<InterfaceMapping> GetAllInterfaceMaps(this Type type)
        {
            return type.GetTypeInfo()
                .ImplementedInterfaces
                .Select(type.GetInterfaceMap);
        }
    }
}
