using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum LanguageMissingExportOption
    {
        Ignore = 0, 
        Create = 1
    }

    public static class LanguageMissingExportOptionExtension
    {
        public static bool IsCreate(this LanguageMissingExportOption option)
        {
            return option == LanguageMissingExportOption.Create;
        }
    }
}