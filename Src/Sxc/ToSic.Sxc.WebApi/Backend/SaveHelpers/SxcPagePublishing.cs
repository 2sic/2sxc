using ToSic.Eav.Security;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Internal.Publishing;

namespace ToSic.Sxc.Backend.SaveHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
    ) => Log.Func<Dictionary<Guid, int>>(() =>
    {
        var allowWriteLive = permCheck.UserMayOnAll(GrantSets.WritePublished);
        var forceDraft = !allowWriteLive;
        Log.A($"allowWrite: {allowWriteLive} forceDraft: {forceDraft}");

        // list of saved IDs
        Dictionary<Guid, int> postSaveIds = null;

        // The internal call which will be used further down
        var appIdentity = appsCatalog.AppIdentity(appId);
        var groupList = contentGroupList.Init(appIdentity/*, Context.UserMayEdit*/);

        Dictionary<Guid, int> SaveAndSaveGroupsInnerCall(Func<bool, Dictionary<Guid, int>> call,
            bool forceSaveAsDraft)
        {
            var ids = call.Invoke(forceSaveAsDraft);
            // now assign all content-groups as needed
            groupList.IfChangesAffectListUpdateIt(blockOrNull, items, ids);
            return ids;
        }


        // use dnn versioning if partOfPage
        if (partOfPage)
        {
            Log.A("partOfPage - save with publishing");
            var versioning = pagePublishing;
            versioning.DoInsidePublishing(Context,
                args => postSaveIds = SaveAndSaveGroupsInnerCall(internalSaveMethod, forceDraft));
        }
        else
        {
            Log.A("partOfPage false, save without publishing");
            postSaveIds = SaveAndSaveGroupsInnerCall(internalSaveMethod, forceDraft);
        }

        Log.A(Log.Try(() =>
            $"post save IDs: {string.Join(",", postSaveIds.Select(psi => psi.Key + "(" + psi.Value + ")"))}"));
        return postSaveIds;
    });
}