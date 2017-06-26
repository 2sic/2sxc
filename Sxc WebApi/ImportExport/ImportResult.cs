using System.Collections.Generic;
using ExportImportMessage = ToSic.Eav.Persistence.Logging.ExportImportMessage;

namespace ToSic.SexyContent.WebApi.ImportExport
{
    public class ImportResult
    {
        public bool Succeeded;

        public List<ExportImportMessage> Messages;
    }

}