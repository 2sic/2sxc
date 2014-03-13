using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum EntityClearImportOption
    {
        None,
        All
    }

    public static class EntityClearImportOptionExtension
    {
        public static bool IsNone(this EntityClearImportOption option)
        {
            return option == EntityClearImportOption.None;
        }

        public static bool IsAll(this EntityClearImportOption option)
        {
            return option == EntityClearImportOption.All;
        }
    }
}