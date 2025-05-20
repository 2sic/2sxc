using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Adam.Work.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkRename<TFolderId, TFileId>(AdamWorkBase<TFolderId, TFileId>.MyServices services)
    : AdamWorkBase<TFolderId, TFileId>(services, "Adm.TrnRen")
{
    public bool Rename(string parentSubfolder, bool isFolder, AdamAssetIdentifier folderId, AdamAssetIdentifier fileId, string newName)
    {
        var l = Log.Fn<bool>();

        if (!AdamContext.Security.UserIsPermittedOnField(GrantSets.WriteSomething, out var exp))
            throw l.Ex(exp);

        // check that if the user should only see drafts, he doesn't see items of published data
        if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out var permissionException))
            throw l.Ex(permissionException);

        // try to see if we can get into the subfolder - will throw error if missing
        var parent = AdamContext.AdamRoot.Folder(parentSubfolder, false);

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

        return l.ReturnTrue("rename complete");
    }
}