using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Decorators
{
    public class NotDecoratedAttributeChecker<T>
    {
        private readonly object _decorated;

        public NotDecoratedAttributeChecker(object decorated)
        {
            _decorated = decorated;
        }

        public bool IsFiltered(MethodBase targetMethod)
        {
            var implementedTargetMethod = _decorated.GetType().GetMethods().Single(x => SameSignature(x, targetMethod));
            var attribute = implementedTargetMethod.GetCustomAttributes<NotDecoratedAttribute>();

            return attribute.Any(x => x.DecoratorType == null || x.DecoratorType == typeof(T));
        }

        private static bool SameSignature(MethodBase a, MethodBase b)
        {
            return a.Name.Equals(b.Name) &&
                   GetParameterTypes(a).SequenceEqual(GetParameterTypes(b));
        }

        private static IEnumerable<Type> GetParameterTypes(MethodBase methodBase)
        {
            return methodBase.GetParameters().Select(x => x.ParameterType);
        }
    }
}
