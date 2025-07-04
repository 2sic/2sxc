﻿using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkDelete(AdamWorkBase.MyServices services)
    : AdamWorkBase(services, "Adm.TrnDel")
{
    public bool Delete(string parentSubfolder, bool isFolder, AdamAssetIdentifier folderId, AdamAssetIdentifier fileId)
    {
        var l = Log.Fn<bool>();
        if (AdamContext.Security.UserNotPermittedOnField(GrantSets.DeleteSomething, out var exp))
            throw l.Ex(exp);

        // check that if the user should only see drafts, he doesn't see items of published data
        if (AdamContext.Security.UserIsRestrictedOrItemIsNotDraft(AdamContext.ItemGuid, out var permissionException))
            throw l.Ex(permissionException);

        // try to see if we can get into the subfolder - will throw error if missing
        var parent = AdamContext.AdamRoot.Folder(parentSubfolder, false);

        var fs = AdamContext.AdamManager.AdamFs;
        if (isFolder)
        {
            var target = fs.GetFolder(folderId);
            VerifySecurityAndStructure(parent, target, "can't delete folder");
            fs.Delete(target);
        }
        else
        {
            var target = fs.GetFile(fileId);
            VerifySecurityAndStructure(parent, target, "can't delete file");
            fs.Delete(target);
        }

        return l.ReturnTrue("delete complete");

    }

}