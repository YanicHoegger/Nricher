using System;

namespace Decorators
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Property)]
    public class NotDecoratedAttribute : Attribute
    {
        public Type? DecoratorType { get; }

        public NotDecoratedAttribute()
        {
        }

        public NotDecoratedAttribute(Type decoratorType)
        {
            DecoratorType = decoratorType;
        }
    }
}
