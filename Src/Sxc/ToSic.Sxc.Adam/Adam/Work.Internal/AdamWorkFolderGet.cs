using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Adam.Work.Internal;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkFolderGet<TFolderId, TFileId>(
    AdamWorkBase<AdamWorkFolderGet<TFolderId, TFileId>, TFolderId, TFileId>.MyServices services)
    : AdamWorkBase<AdamWorkFolderGet<TFolderId, TFileId>, TFolderId, TFileId>(services, "Adm.TrnFld")
{
    public AdamFolderFileSet<TFolderId, TFileId> Folder(string parentSubfolder, string newFolder)
    {
        var logCall = Log.Fn<AdamFolderFileSet<TFolderId, TFileId>>($"get folders for subfld:{parentSubfolder}, new:{newFolder}");
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

        return logCall.ReturnAsOk(ItemsInFieldNew(parentSubfolder));
    }
}