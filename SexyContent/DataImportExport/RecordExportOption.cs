using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.DataImportExport
{
    public enum RecordExportOption
    {
        Blank,
        All
    }

    public static class RecordExportOptionExtension
    {
        public static bool IsBlank(this RecordExportOption option)
        {
            return option == RecordExportOption.Blank;
        }        
    }
}