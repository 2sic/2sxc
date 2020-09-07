using System;
using JetBrains.Annotations;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Adam
{
    internal partial class AdamBackend
    {
        internal bool Delete(IBlock block, int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id,
    bool usePortalRoot)
        {
            Log.Add(
                $"delete from a:{appId}, i:{guid}, field:{field}, file:{id}, subf:{subfolder}, isFld:{isFolder}, useRoot:{usePortalRoot}");
            var state = new AdamState(block, appId, contentType, field, guid, usePortalRoot, Log);
            if (!state.Security.UserIsPermittedOnField(GrantSets.DeleteSomething, out var exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!state.Security.UserIsNotRestrictedOrItemIsDraft(guid, out var permissionException))
                throw permissionException;

            // try to see if we can get into the subfolder - will throw error if missing
            var parent = state.ContainerContext.Folder(subfolder, false);

            var fs = state.AdamAppContext.EnvironmentFs;
            if (isFolder)
            {
                var target = fs.GetFolder(id);
                VerifySecurityAndStructure(state, parent, target, id, usePortalRoot, "can't delete folder");
                fs.Delete(target);
            }
            else
            {
                var target = fs.GetFile(id);
                VerifySecurityAndStructure(state, parent, target, target.FolderId, usePortalRoot, "can't delete file");
                fs.Delete(target);
            }

            Log.Add("delete complete");
            return true;
        }

    }
}
