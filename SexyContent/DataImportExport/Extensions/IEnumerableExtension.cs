using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport.Extensions
{
    public static class IEnumerableExtension
    {
        public static int IndexOf<T>(this IEnumerable<T> list, T item)
        {
            return list.TakeWhile(i => !i.Equals(item)).Count();
        }
    }
}