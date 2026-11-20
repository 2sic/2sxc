using ToSic.Eav.Data.Publishing.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.SaveHelpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcPagePublishing(AppWorkChain<ContentGroupList> contentGroupList, IDataPublishingService pagePublishing)
    : ServiceWithSetup<IAppWorkContext>("Sxc.PgPubl", connect: [contentGroupList, pagePublishing])
{

    internal async Task<Dictionary<Guid, int>> SaveInPagePublishing(
        IContextOfApp context,
        IBlock? blockOrNull,
        int appId,
        List<BundleWithHeader<IEntity>> items,
        bool partOfPage,
        Func<bool, Dictionary<Guid, int>> internalSaveMethod,
        IMultiPermissionCheck permCheck
    )
    {
        var l = Log.Fn<Dictionary<Guid, int>>();
        var allowWriteLive = permCheck.EnsureAll(GrantSets.WritePublished).Allowed;
        var forceDraft = !allowWriteLive;
        l.A($"allowWrite: {allowWriteLive} forceDraft: {forceDraft}");

        // list of saved IDs
        Dictionary<Guid, int>? postSaveIds = null;

        // use dnn versioning if partOfPage
        if (partOfPage)
        {
            l.A("partOfPage - save with publishing");
            pagePublishing.DoInsidePublishing(options =>
            {
                postSaveIds = SaveAndSaveGroupsInnerCall(internalSaveMethod, options.ForceDraft || forceDraft)
                    .GetAwaiter()
                    .GetResult();
            });
        }
        else
        {
            l.A("partOfPage false, save without publishing");
            postSaveIds = await SaveAndSaveGroupsInnerCall(internalSaveMethod, forceDraft);
        }

        var logIds = l.Try(() => string.Join(",", postSaveIds!.Select(psi => $"{psi.Key}({psi.Value})")));
        return l.Return(postSaveIds!, $"post save IDs: {logIds}");

        async Task<Dictionary<Guid, int>> SaveAndSaveGroupsInnerCall(Func<bool, Dictionary<Guid, int>> call, bool forceSaveAsDraft)
        {
            // The internal call which will be used further down
            var groupList = contentGroupList.New(MyOptions);

            var ids = call.Invoke(forceSaveAsDraft);
            // now assign all content-groups as needed
            await groupList.IfChangesAffectListUpdateIt(blockOrNull, items, ids);
            return ids;
        }
    }
}