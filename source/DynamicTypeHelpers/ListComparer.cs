using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicTypeHelpers
{
    public class ListComparer<T> : IEqualityComparer<List<T>>
    {
        public bool Equals(List<T>? x, List<T>? y)
        {
            if (ReferenceEquals(x, y)) 
                return true;
            if (x is null) 
                return false;
            if (y is null) 
                return false;
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (x.GetType() != y.GetType()) 
                return false;

            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<T> obj)
        {
            if (obj == null) 
                throw new ArgumentNullException(nameof(obj));

            unchecked
            {
                return obj.Aggregate(19, (current, item) => item != null ? current * 31 + item.GetHashCode() : current);
            }
        }
    }
}
