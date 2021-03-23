using System;

namespace Decorators
{
    //TODO: Create test for multiple decorations when using attribute
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
