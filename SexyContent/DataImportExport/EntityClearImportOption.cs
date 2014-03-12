using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum EntityClearImportOption
    {
        None,
        Obsolete
    }

    public static class EntityClearImportOptionExtension
    {
        public static bool IsNone(this EntityClearImportOption option)
        {
            return option == EntityClearImportOption.None;
        }

        public static bool IsWasted(this EntityClearImportOption option)
        {
            return option == EntityClearImportOption.Obsolete;
        }
    }
}