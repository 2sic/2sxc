using System;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamTransRename<TFolderId, TFileId> : AdamTransactionBase<AdamTransRename<TFolderId, TFileId>, TFolderId, TFileId>
    {
        public AdamTransRename(Lazy<AdamContext<TFolderId, TFileId>> adamState, IContextResolver ctxResolver) : base(adamState, ctxResolver, "Adm.TrnRen") { }

        public bool Rename(string parentSubfolder, bool isFolder, TFolderId folderId, TFileId fileId, string newName)
        {
            Log.A($"");

            if (!AdamContext.Security.UserIsPermittedOnField(GrantSets.WriteSomething, out var exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out var permissionException))
                throw permissionException;

            // try to see if we can get into the subfolder - will throw error if missing
            var parent = AdamContext.AdamRoot.Folder(parentSubfolder, false); // as IFolder<TFolderId, TFileId>;

            var fs = AdamContext.AdamManager.AdamFs;
            if (isFolder)
            {
                var target = fs.GetFolder(folderId);
                VerifySecurityAndStructure(parent, target, "Can't rename folder");
                fs.Rename(target, newName);
            }
            else
            {
                var target = fs.GetFile(fileId);
                VerifySecurityAndStructure(parent, target, "Can't rename file");

                // never allow to change the extension
                if (target.Extension != newName.Split('.').Last())
                    newName += "." + target.Extension;
                fs.Rename(target, newName);
            }

            Log.A("rename complete");
            return true;
        }
    }
}
