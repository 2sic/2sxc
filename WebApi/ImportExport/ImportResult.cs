using System.Collections.Generic;
using ToSic.Eav.ImportExport.Logging;

namespace ToSic.SexyContent.WebApi
{
    public class ImportResult
    {
        public bool Succeeded;

        public List<ExportImportMessage> Messages;
    }

}