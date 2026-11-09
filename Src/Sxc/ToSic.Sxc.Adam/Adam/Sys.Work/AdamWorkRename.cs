using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkRename(Generator<AdamContext, AdamContext.Options> adamContextGen, AdamGenericHelper adamGenericHelper)
    : ServiceWithSetup<AdamContext.Options>("Adm.TrnRen", connect: [adamContextGen])
{
    private AdamContext AdamContext => field ??= adamContextGen.New(MyOptions);

    public bool Rename(string parentSubfolder, bool isFolder, AdamAssetIdentifier folderId, AdamAssetIdentifier fileId, string newName)
    {
        var l = Log.Fn<bool>();
        var adamCtx = AdamContext;
        if (adamCtx.Security.UserNotPermittedOnField(GrantSets.WriteSomething, out var exp))
            throw l.Ex(exp);

        // check that if the user should only see drafts, he doesn't see items of published data
        if (adamCtx.Security.UserIsRestrictedOrItemIsNotDraft(adamCtx.ItemGuid, out var permissionException))
            throw l.Ex(permissionException);

        // try to see if we can get into the subfolder - will throw error if missing
        var parent = adamCtx.AdamRoot.Folder(parentSubfolder, false);

        var fs = adamCtx.AdamManager.AdamFs;
        if (isFolder)
        {
            var target = fs.GetFolder(folderId);
            AdamWorkSecHelper.VerifySecurityAndStructure(adamCtx, adamGenericHelper, parent, target, "Can't rename folder");
            fs.Rename(target, newName);
        }
        else
        {
            var target = fs.GetFile(fileId);
            AdamWorkSecHelper.VerifySecurityAndStructure(adamCtx, adamGenericHelper, parent, target, "Can't rename file");

            // never allow to change the extension
            if (target.Extension != newName.Split('.').Last())
                newName += "." + target.Extension;
            fs.Rename(target, newName);
        }

        return l.ReturnTrue("rename complete");
    }
}