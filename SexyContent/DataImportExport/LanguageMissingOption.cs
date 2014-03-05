using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum LanguageMissingOption
    {
        Ignore = 0, 
        Create = 1
    }

    public static class LanguageMissingOptionExtension
    {
        public static bool IsCreate(this LanguageMissingOption option)
        {
            return option == LanguageMissingOption.Create;
        }
    }
}