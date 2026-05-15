using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Targets;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sxc.Backend.SaveHelpers;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityCleanupRequest(ContentGroupList contentGroupList, ITargetTypeService mdTargetTypes)
    : ServiceBase("UoW.AddCtx", connect: [contentGroupList, mdTargetTypes]),
        ILowCodeAction<List<ItemIdentifier>, List<ItemIdentifier>>
{
    // Note: reworked this 2026-05-15 2dm to make the objects immutable, hope no side effects #ImmutableIsTheNewBlack
    public async Task<ActionData<List<ItemIdentifier>>> Run(LowCodeActionContext actionCtx, ActionData<List<ItemIdentifier>> data)
    {
        var l = Log.Fn<List<ItemIdentifier>>();

        var items = data.Data;
        var appReader = actionCtx.Get<IAppReader>(EditLoadContextConstants.AppReader);
        var cglHelper = contentGroupList.Init(appReader.PureIdentity());
        items = cglHelper.ConvertGroup(items);
        items = cglHelper.ConvertListIndexToId(items);
        var final = TryToAutoFindMetadataSingleton(items, appReader.Metadata);


        return ActionData.Create(l.Return(final));
    }


    /// <summary>
    /// new 2020-12-08 - correct entity-id with lookup of existing if marked as singleton
    /// </summary>
    // ReSharper disable once UnusedMethodReturnValue.Local
    private List<ItemIdentifier> TryToAutoFindMetadataSingleton(List<ItemIdentifier> list, IMetadataSource appMdSource)
    {
        var l = Log.Fn<List<ItemIdentifier>>();

        var result = list
            .Select(header =>
            {
                // Check if it applies
                var mustUpdate = header.For?.Singleton == true && header.ContentTypeName.HasValue();
                if (!mustUpdate)
                    return header;

                l.A("Found an entity with the auto-lookup marker");
                // try to find metadata for this
                var mdFor = header.For;
                // #TargetTypeIdInsteadOfTarget
                var type = mdFor!.TargetType != 0
                    ? mdFor.TargetType
                    : mdTargetTypes.GetId(mdFor.Target!);
                var mds = mdFor.Guid != null
                    ? appMdSource.GetMetadata(type, mdFor.Guid.Value, header.ContentTypeName)
                    : mdFor.Number != null
                        ? appMdSource.GetMetadata(type, mdFor.Number.Value, header.ContentTypeName)
                        : appMdSource.GetMetadata(type, mdFor.String, header.ContentTypeName);

                var mdList = mds.ToArray();
                if (mdList.Length > 1)
                {
                    l.A($"Warning - looking for best metadata but found too many {mdList.Length}, will use first");
                    // must now sort by ID otherwise the order may be different after a few save operations
                    mdList = [.. mdList.OrderBy(e => e.EntityId)];
                }

                return header with { EntityId = !mdList.Any() ? 0 : mdList.First().EntityId };
            })
            .ToList();

        return l.Return(result);
    }
}
