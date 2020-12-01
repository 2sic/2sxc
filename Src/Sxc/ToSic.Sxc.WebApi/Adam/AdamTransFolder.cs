using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamTransFolder<TFolderId, TFileId> : AdamTransactionBase<AdamTransFolder<TFolderId, TFileId>, TFolderId, TFileId>
    {
        public AdamTransFolder(Lazy<AdamState<TFolderId, TFileId>> adamState, IContextOfApp context) : base(adamState, context, "Adm.TrnFld") { }
        internal IList<AdamItemDto> Folder(string parentSubfolder, string newFolder)
        {
            var logCall = Log.Call<IList<AdamItemDto>>($"get folders for subfld:{parentSubfolder}, new:{newFolder}");
            if (State.Security.UserIsRestricted && !State.Security.FieldPermissionOk(GrantSets.ReadSomething))
                return null;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            var folder = State.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            if (!string.IsNullOrEmpty(parentSubfolder))
                folder = State.ContainerContext.Folder(parentSubfolder, false);

            // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
            if (State.UseSiteRoot && !State.Security.CanEditFolder(folder))
                throw HttpException.PermissionDenied("can't create new folder - permission denied");

            var newFolderPath = string.IsNullOrEmpty(parentSubfolder)
                ? newFolder
                : Path.Combine(parentSubfolder, newFolder).Replace("\\", "/");

            // now access the subfolder, creating it if missing (which is what we want
            State.ContainerContext.Folder(newFolderPath, true);

            return logCall("ok", ItemsInField(parentSubfolder));
        }
    }
}
