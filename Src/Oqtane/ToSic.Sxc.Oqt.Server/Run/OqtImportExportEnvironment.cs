using System;
using System.Collections.Generic;
using ToSic.Eav.Persistence.Logging;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtImportExportEnvironment: ImportExportEnvironmentBase
    {
        public OqtImportExportEnvironment(Dependencies dependencies) : base(dependencies, $"{OqtConstants.OqtLogPrefix}.IExEnv") { }

        public override List<Message> TransferFilesToSite(string sourceFolder, string destinationFolder) 
            => throw new NotImplementedException();

        public override Version TenantVersion => typeof(OqtImportExportEnvironment).Assembly.GetName().Version;

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
