using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum ResourceReferenceOption
    {
        Link = 0,
        Resolve = 1
    }

    public static class ResourceReferenceOptionExtension
    {
        public static bool IsResolve(this ResourceReferenceOption option)
        {
            return option == ResourceReferenceOption.Resolve;
        }
    }
}