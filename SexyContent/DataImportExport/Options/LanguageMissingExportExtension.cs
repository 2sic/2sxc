using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class LanguageMissingExportExtension
    {
        public static bool IsCreate(this LanguageMissingExport option)
        {
            return option == LanguageMissingExport.Create;
        }
    }
}