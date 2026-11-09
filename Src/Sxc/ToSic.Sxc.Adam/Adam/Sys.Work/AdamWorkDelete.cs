using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamWorkDelete(AdamGenericHelper adamGenericHelper) : ServiceWithSetup<AdamContext>("Adm.TrnDel")
{
    public bool Delete(string parentSubfolder, bool isFolder, AdamAssetIdentifier folderId, AdamAssetIdentifier fileId)
    {
        var l = Log.Fn<bool>();
        var adamCtx = MyOptions;
        if (adamCtx.Security.UserNotPermittedOnField(GrantSets.DeleteSomething, out var exp))
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
            AdamWorkSecHelper.VerifySecurityAndStructure(adamCtx, adamGenericHelper, parent, target, "can't delete folder");
            fs.Delete(target);
        }
        else
        {
            var target = fs.GetFile(fileId);
            AdamWorkSecHelper.VerifySecurityAndStructure(adamCtx, adamGenericHelper, parent, target, "can't delete file");
            fs.Delete(target);
        }

        return l.ReturnTrue("delete complete");

    }

}

// Experimental, not in use
//public class AdamWorkGen<T>(Generator<AdamContext, AdamContext.Options> adamContextGen, Generator<T, AdamContext> adamGen) where T: IServiceWithSetup<AdamContext>
//{
//    public T New(AdamContext.Options options)
//    {
//        var adamCtx = adamContextGen.New(options);
//        var svc = adamGen.New(adamCtx);
//        return svc;
//    }
//}
