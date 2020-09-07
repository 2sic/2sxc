using System;
using System.Linq;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Adam
{
    internal partial class AdamBackend
    {
        internal bool Rename(IBlock block, int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id,
    string newName, bool usePortalRoot)
        {
            Log.Add(
                $"rename a:{appId}, i:{guid}, field:{field}, subf:{subfolder}, isfld:{isFolder}, new:{newName}, useRoot:{usePortalRoot}");

            var state = new AdamState(block, appId, contentType, field, guid, usePortalRoot, Log);
            if (!state.Security.UserIsPermittedOnField(GrantSets.WriteSomething, out var exp))
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
                VerifySecurityAndStructure(state, parent, target, target.Id, usePortalRoot, "Can't rename folder");
                fs.Rename(target, newName);
            }
            else
            {
                var target = fs.GetFile(id);
                VerifySecurityAndStructure(state, parent, target, target.FolderId, usePortalRoot, "Can't rename file");

                // never allow to change the extension
                if (target.Extension != newName.Split('.').Last())
                    newName += "." + target.Extension;
                fs.Rename(target, newName);
            }

            Log.Add("rename complete");
            return true;
        }
    }
}
