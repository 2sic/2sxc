using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum LanguageReferenceOption
    {
        Link = 0,
        Resolve = 2
    }

    public static class LanguageReferenceOptionExtension
    {
        public static bool IsLink(this LanguageReferenceOption option)
        {
            return option == LanguageReferenceOption.Link;
        }

        public static bool IsResolve(this LanguageReferenceOption option)
        {
            return option == LanguageReferenceOption.Resolve;
        }
    }
}