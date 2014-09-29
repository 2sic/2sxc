using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport.Options
{
    public static class RecordExportExtension
    {
        public static bool IsBlank(this RecordExport option)
        {
            return option == RecordExport.Blank;
        }
    }
}