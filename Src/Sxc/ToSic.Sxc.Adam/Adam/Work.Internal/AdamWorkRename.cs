namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkRename<TFolderId, TFileId>(
    AdamWorkBase<TFolderId, TFileId>.MyServices services)
    : AdamWorkBase<TFolderId, TFileId>(services, "Adm.TrnRen")
{
    public bool Rename(string parentSubfolder, bool isFolder, TFolderId folderId, TFileId fileId, string newName)
    {
        Log.A("");

        if (!AdamContext.Security.UserIsPermittedOnField(GrantSets.WriteSomething, out var exp))
            throw exp;

        // check that if the user should only see drafts, he doesn't see items of published data
        if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out var permissionException))
            throw permissionException;

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

        Log.A("rename complete");
        return true;
    }
}