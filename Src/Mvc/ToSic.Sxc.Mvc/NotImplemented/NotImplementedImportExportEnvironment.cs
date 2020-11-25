using System;
using System.Collections.Generic;
using ToSic.Eav.Persistence.Logging;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Mvc.NotImplemented
{
    public class NotImplementedImportExportEnvironment: ImportExportEnvironmentBase
    {
        public NotImplementedImportExportEnvironment(Dependencies dependencies) : base(dependencies, "Mvc.IExEnv") { }

        public override List<Message> TransferFilesToSite(string sourceFolder, string destinationFolder) 
            => throw new NotImplementedException();

        public override Version TenantVersion => typeof(NotImplementedImportExportEnvironment).Assembly.GetName().Version;

        public override void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap)
        {
            throw new NotImplementedException();
        }

        public override void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog)
        {
            throw new NotImplementedException();
        }
    }
}
