using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static partial class Extensions
    {
        public static bool IsAny<T>(this IEnumerable<T> value)
        {
            return value != null && value.Any();
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> value, Action<T> action)
        {
            foreach (var obj in value)
                action(obj);

            return value;
        }
    }
}