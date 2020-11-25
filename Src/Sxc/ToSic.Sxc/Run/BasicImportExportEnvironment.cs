using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Persistence.Logging;

namespace ToSic.Sxc.Run
{
    public class BasicImportExportEnvironment: ImportExportEnvironmentBase
    {
        public BasicImportExportEnvironment(Dependencies dependencies) : base(dependencies, $"{LogNames.NotImplemented}.IExEnv") { }

        public override List<Message> TransferFilesToSite(string sourceFolder, string destinationFolder)
        {
            // don't do anything
            return new List<Message>();
        }

        public override Version TenantVersion => new Version(0,0,0);

        public override void MapExistingFilesToImportSet(Dictionary<int, string> filesAndPaths, Dictionary<int, int> fileIdMap)
        {
            // don't do anything
        }

        public override void CreateFoldersAndMapToImportIds(Dictionary<int, string> foldersAndPath, Dictionary<int, int> folderIdCorrectionList, List<Message> importLog)
        {
            // don't do anything
        }
    }
}
