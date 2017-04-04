using System.Collections.Generic;
using ToSic.Eav.ImportExport;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent.WebApi
{
    public class ImportResult
    {
        public bool Succeeded;

        public List<ExportImportMessage> Messages;

        //public ImportResult()
        //{
        //    Messages = new List<ExportImportMessage>();
        //}
    }

}