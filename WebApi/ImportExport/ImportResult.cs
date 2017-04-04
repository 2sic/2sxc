using System.Collections.Generic;
using ToSic.Eav.ImportExport;

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