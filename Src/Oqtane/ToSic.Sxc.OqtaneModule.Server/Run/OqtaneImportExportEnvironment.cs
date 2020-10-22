using System;
using System.Collections.Generic;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    public class OqtaneImportExportEnvironment: ImportExportEnvironmentBase
    {
        public OqtaneImportExportEnvironment(IEnvironment environment, ITenant tenant) 
            : base(environment, tenant, "Mvc.IExEnv")
        {
        }

        public override List<Message> TransferFilesToTenant(string sourceFolder, string destinationFolder) 
            => throw new NotImplementedException();

        public override Version TenantVersion => typeof(OqtaneImportExportEnvironment).Assembly.GetName().Version;

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
