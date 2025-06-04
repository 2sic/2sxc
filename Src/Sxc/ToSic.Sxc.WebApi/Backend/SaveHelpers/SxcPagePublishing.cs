using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.SaveHelpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcPagePublishing(ContentGroupList contentGroupList, IPagePublishing pagePublishing, IAppsCatalog appsCatalog)
    : SaveHelperBase("Sxc.PgPubl", connect: [contentGroupList, pagePublishing, appsCatalog])
{

    internal Dictionary<Guid, int> SaveInPagePublishing(
        IBlock blockOrNull,
        int appId,
        List<BundleWithHeader<IEntity>> items,
        bool partOfPage,
        Func<bool, Dictionary<Guid, int>> internalSaveMethod,
        IMultiPermissionCheck permCheck
    )
    {
        var l = Log.Fn<Dictionary<Guid, int>>();
        var allowWriteLive = permCheck.UserMayOnAll(GrantSets.WritePublished);
        var forceDraft = !allowWriteLive;
        l.A($"allowWrite: {allowWriteLive} forceDraft: {forceDraft}");

        // list of saved IDs
        Dictionary<Guid, int> postSaveIds = null;

        // The internal call which will be used further down
        var appIdentity = appsCatalog.AppIdentity(appId);
        var groupList = contentGroupList.Init(appIdentity);

        Dictionary<Guid, int> SaveAndSaveGroupsInnerCall(Func<bool, Dictionary<Guid, int>> call, bool forceSaveAsDraft)
        {
            var ids = call.Invoke(forceSaveAsDraft);
            // now assign all content-groups as needed
            groupList.IfChangesAffectListUpdateIt(blockOrNull, items, ids);
            return ids;
        }


        // use dnn versioning if partOfPage
        if (partOfPage)
        {
            l.A("partOfPage - save with publishing");
            pagePublishing.DoInsidePublishing(Context, _ => postSaveIds = SaveAndSaveGroupsInnerCall(internalSaveMethod, forceDraft));
        }
        else
        {
            l.A("partOfPage false, save without publishing");
            postSaveIds = SaveAndSaveGroupsInnerCall(internalSaveMethod, forceDraft);
        }

        var logIds = l.Try(() => string.Join(",", postSaveIds.Select(psi => $"{psi.Key}({psi.Value})")));
        return l.Return(postSaveIds, $"post save IDs: {logIds}");
    }
}