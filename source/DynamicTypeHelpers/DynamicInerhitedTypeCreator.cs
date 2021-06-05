using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Nricher.DynamicTypeHelpers
{
    //Need to be static since we can not create the same type twice
    public static class DynamicInheritedTypeCreator
    {
        //TODO: Needs a lock for multi threading
        private static readonly Dictionary<Type, Type> AlreadyCreatedInheritanceTypes = new();

        public static Type CreateInheritedType([NotNull] Type type)
        {
            CheckToInheritType(type);

            if (AlreadyCreatedInheritanceTypes.ContainsKey(type))
                return AlreadyCreatedInheritanceTypes[type];

            var typeBuilder = ModuleBuilderCreator.ModuleBuilder.DefineType($"{type.Name}InheritanceProxy", TypeAttributes.Class | TypeAttributes.Public, type);

            DefineConstructor(typeBuilder, type);

            var inheritedType = typeBuilder.CreateType();
            Debug.Assert(inheritedType != null, nameof(inheritedType) + " != null");

            AlreadyCreatedInheritanceTypes[type] = inheritedType;

            return inheritedType;
        }

        private static void CheckToInheritType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsClass || type.IsAbstract)
                throw new ArgumentException($"To create inherited type {type.Name} can not be abstract or interface");
            if (type.IsSealed)
                throw new ArgumentException($"Type {type.Name} can not be sealed");
        }

        private static void DefineConstructor(TypeBuilder typeBuilder, Type type)
        {
            var constructor = GetConstructor(type);

            var parameterInfos = constructor.GetParameters();

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                parameterInfos.Select(x => x.ParameterType).ToArray());

            var ilGenerator = constructorBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0); // push "this" onto stack.

            for (var i = 0; i < parameterInfos.Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);
            }

            ilGenerator.Emit(OpCodes.Call, constructor);
            ilGenerator.Emit(OpCodes.Ret);
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            if (constructors.Count(x => x.IsPublic) != 1)
                throw new ArgumentException($"Type {type.Name} can not have multiple public constructors");

            return constructors.Single(x => x.IsPublic);
        }
    }
}
