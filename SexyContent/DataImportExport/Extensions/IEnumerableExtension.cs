using System.Collections.Generic;
using System.Linq;

namespace ToSic.Eav.ImportExport.Refactoring.Extensions
{
    internal static class IEnumerableExtension
    {
        public static int IndexOf<T>(this IEnumerable<T> list, T item)
        {
            return list.TakeWhile(i => !i.Equals(item)).Count();
        }
    }
}