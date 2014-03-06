using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum LanguageReferenceExportOption
    {
        Link = 0,
        Resolve = 2
    }

    public static class LanguageReferenceExportOptionExtension
    {
        public static bool IsLink(this LanguageReferenceExportOption option)
        {
            return option == LanguageReferenceExportOption.Link;
        }

        public static bool IsResolve(this LanguageReferenceExportOption option)
        {
            return option == LanguageReferenceExportOption.Resolve;
        }
    }
}