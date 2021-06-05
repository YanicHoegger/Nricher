using System;
using System.Linq;
using System.Reflection;

namespace Nricher.Decorators
{
    public class NotDecoratedAttributeChecker<T>
    {
        private readonly object _decorated;

        public NotDecoratedAttributeChecker(object decorated)
        {
            _decorated = decorated;
        }

        public bool IsFiltered(MethodInfo targetMethod)
        {
            var implementedTargetMethod = _decorated.GetType().GetMethods().Single(x => x.SameMethod(targetMethod));
            var attributes = implementedTargetMethod.GetCustomAttributes<NotDecoratedAttribute>();

            return attributes.Any(x => x.DecoratorType == null || IsSameTypeIgnoringGenerics(x.DecoratorType));
        }

        //We want to check for if same type as decorator but ignore its generics
        private static bool IsSameTypeIgnoringGenerics(Type decoratingType)
        {
            if (!decoratingType.IsGenericType)
                return decoratingType == typeof(T);
            if (!typeof(T).IsGenericType)
                return false;

            return decoratingType.GetGenericTypeDefinition() == typeof(T).GetGenericTypeDefinition();
        }
    }
}
