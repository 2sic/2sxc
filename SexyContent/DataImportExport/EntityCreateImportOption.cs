using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum EntityCreateImportOption
    {
        Create,
        Overwrite,
        CreateOverwrite
    }

    public static class EntityCreateImportOptionExtension
    {
        public static bool IsCreate(this EntityCreateImportOption option)
        {
            return option == EntityCreateImportOption.Create;
        }

        public static bool IsOverwrite(this EntityCreateImportOption option)
        {
            return option == EntityCreateImportOption.Overwrite;
        }

        public static bool IsCreateOverwrite(this EntityCreateImportOption option)
        {
            return option == EntityCreateImportOption.CreateOverwrite;
        }
    }
}