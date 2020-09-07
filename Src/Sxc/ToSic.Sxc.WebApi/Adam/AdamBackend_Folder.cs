using System;
using System.Collections.Generic;
using System.IO;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Adam
{
    internal partial class AdamBackend
    {
        internal IList<AdamItemDto> Folder(IBlock block, int appId, string contentType, Guid guid, string field, string subfolder, string newFolder,
            bool usePortalRoot)
        {
            var logCall = Log.Call<IList<AdamItemDto>>($"get folders for a:{appId}, i:{guid}, field:{field}, subfld:{subfolder}, new:{newFolder}, useRoot:{usePortalRoot}");
            var state = new AdamState(block, appId, contentType, field, guid, usePortalRoot, Log);
            if (state.Security.UserIsRestricted && !state.Security.FieldPermissionOk(GrantSets.ReadSomething))
                return null;

            // get root and at the same time auto-create the core folder in case it's missing (important)
            var folder = state.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            if (!string.IsNullOrEmpty(subfolder))
                folder = state.ContainerContext.Folder(subfolder, false);

            // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
            if (usePortalRoot && !state.Security.CanEditFolder(folder.Id))
                throw HttpException.PermissionDenied("can't create new folder - permission denied");

            var newFolderPath = string.IsNullOrEmpty(subfolder)
                ? newFolder
                : Path.Combine(subfolder, newFolder).Replace("\\", "/");

            // now access the subfolder, creating it if missing (which is what we want
            state.ContainerContext.Folder(newFolderPath, true);

            return logCall("ok", ItemsInField(state, guid, subfolder, usePortalRoot));
        }
    }
}
