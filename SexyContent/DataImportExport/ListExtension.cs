using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public static class ListExtension
    {
        public static void Move<T>(this List<T> list, int index, T item)
        {
            list.Remove(item);
            list.Insert(index, item);
        }
    }
}