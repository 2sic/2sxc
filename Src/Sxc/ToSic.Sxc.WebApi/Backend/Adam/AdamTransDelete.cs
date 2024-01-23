namespace ToSic.Sxc.Backend.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamTransDelete<TFolderId, TFileId>(
    AdamTransactionBase<AdamTransDelete<TFolderId, TFileId>, TFolderId, TFileId>.MyServices services)
    : AdamTransactionBase<AdamTransDelete<TFolderId, TFileId>, TFolderId, TFileId>(services, "Adm.TrnDel")
{
    public bool Delete(string parentSubfolder, bool isFolder, TFolderId id, TFileId fileId)
    {
        Log.A($"delete");
        if (!AdamContext.Security.UserIsPermittedOnField(GrantSets.DeleteSomething, out var exp))
            throw exp;

        // check that if the user should only see drafts, he doesn't see items of published data
        if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out var permissionException))
            throw permissionException;

        // try to see if we can get into the subfolder - will throw error if missing
        var parent = AdamContext.AdamRoot.Folder(parentSubfolder, false);

        var fs = AdamContext.AdamManager.AdamFs;
        if (isFolder)
        {
            var target = fs.GetFolder(id);
            VerifySecurityAndStructure(parent, target, "can't delete folder");
            fs.Delete(target);
        }
        else
        {
            var target = fs.GetFile(fileId);
            VerifySecurityAndStructure(parent, target, "can't delete file");
            fs.Delete(target);
        }

        Log.A("delete complete");
        return true;
    }

}