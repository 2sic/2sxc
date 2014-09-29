using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class ResourceReferenceExportExtension
    {
        public static bool IsResolve(this ResourceReferenceExport option)
        {
            return option == ResourceReferenceExport.Resolve;
        }
    }
}