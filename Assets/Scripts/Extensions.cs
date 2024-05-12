using System.Collections.Generic;

namespace WarForTheThrone.Extension
{
    public static class Extensions
    {
        public static void AddIfNotNull<T>(this IList<T> list, T element)
        {
            if (element != null) list.Add(element);
        }
        public static void RemoveIfNotNull<T>(this IList<T> list, T element)
        {
            if (element != null) list.Remove(element);
        }
    }
}