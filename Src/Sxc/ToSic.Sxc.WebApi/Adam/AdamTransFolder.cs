using System.Collections.Generic;
using System.IO;
using ToSic.Lib.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamTransFolder<TFolderId, TFileId> : AdamTransactionBase<AdamTransFolder<TFolderId, TFileId>, TFolderId, TFileId>
    {
        public AdamTransFolder(AdamDependencies<TFolderId, TFileId> dependencies) : base(dependencies, "Adm.TrnFld") { }

        public IList<AdamItemDto> Folder(string parentSubfolder, string newFolder)
        {
            var logCall = Log.Fn<IList<AdamItemDto>>($"get folders for subfld:{parentSubfolder}, new:{newFolder}");
            if (AdamContext.Security.UserIsRestricted && !AdamContext.Security.FieldPermissionOk(GrantSets.ReadSomething))
                return null;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            var folder = AdamContext.AdamRoot.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            if (!string.IsNullOrEmpty(parentSubfolder))
                folder = AdamContext.AdamRoot.Folder(parentSubfolder, false);

            // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
            if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(folder))
                throw HttpException.PermissionDenied("can't create new folder - permission denied");

            var newFolderPath = string.IsNullOrEmpty(parentSubfolder)
                ? newFolder
                : Path.Combine(parentSubfolder, newFolder).Replace("\\", "/");

            // now access the subfolder, creating it if missing (which is what we want
            AdamContext.AdamRoot.Folder(newFolderPath, true);

            return logCall.ReturnAsOk(ItemsInField(parentSubfolder));
        }
    }
}
