using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Targets;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sxc.Backend.SaveHelpers;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityCleanupRequest(ContentGroupList contentGroupList, ITargetTypeService mdTargetTypes)
    : ServiceBase("UoW.AddCtx", connect: [contentGroupList, mdTargetTypes])
{
    public async Task<ActionData<List<ItemIdentifier>>> Run(LowCodeActionContext actionCtx, ActionData<List<ItemIdentifier>> data)
    {
        var l = Log.Fn<List<ItemIdentifier>>();

        var items = data.Data;
        var appReader = actionCtx.Get<IAppReader>(EditLoadContextConstants.AppReader);
        items = contentGroupList.Init(appReader.PureIdentity())
            .ConvertGroup(items)
            .ConvertListIndexToId(items);
        items = TryToAutoFindMetadataSingleton(items, appReader.Metadata);


        return ActionData.Create(l.Return(items));
    }


    /// <summary>
    /// new 2020-12-08 - correct entity-id with lookup of existing if marked as singleton
    /// </summary>
    // ReSharper disable once UnusedMethodReturnValue.Local
    private List<ItemIdentifier> TryToAutoFindMetadataSingleton(List<ItemIdentifier> list, IMetadataSource appMdSource)
    {
        var l = Log.Fn<List<ItemIdentifier>>();
        var headersWithMetadataFor = list
            .Where(header => header.For?.Singleton == true && header.ContentTypeName.HasValue())
            .ToListOpt();

        foreach (var header in headersWithMetadataFor)
        {
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
            header.EntityId = !mdList.Any() ? 0 : mdList.First().EntityId;
        }

        return l.Return(list);
    }
}
