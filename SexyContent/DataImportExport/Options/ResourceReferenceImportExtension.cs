using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class ResourceReferenceImportExtension
    {
        public static bool IsResolve(this ResourceReferenceImport option)
        {
            return option == ResourceReferenceImport.Resolve;
        }
    }
}