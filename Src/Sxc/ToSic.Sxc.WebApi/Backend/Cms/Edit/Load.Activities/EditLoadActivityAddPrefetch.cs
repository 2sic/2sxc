using ToSic.Eav.Data.Processing;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sxc.Adam.Sys.Work;
using ToSic.Sxc.Backend.Adam;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class EditLoadActivityAddPrefetch(
    Generator<HyperlinkBackend> hyperlinkBackend,
    Generator<IAdamPrefetchHelper, AdamWorkOptions> adamTransGetItems,
    EntityPickerApi entityPickerBackend)
    : ServiceBase(SxcLogName + ".Prefetch", connect: [adamTransGetItems, hyperlinkBackend, entityPickerBackend]),
        ILowCodeAction<EditLoadDto, EditLoadDto>
{
    public async Task<ActionData<EditLoadDto>> Run(LowCodeActionContext actionCtx, ActionData<EditLoadDto> result)
    {
        var l = Log.Fn<ActionData<EditLoadDto>>();
        try
        {
            result = result with
            {
                Data = result.Data with
                {
                    Prefetch = TryToPrefectAdditionalData(actionCtx.Get<int>(EditLoadContextConstants.AppId), result.Data)
                }
            };
            return l.Return(result, "prefetched");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(result, "error prefetching");
        }
    }

    public EditPrefetchDto TryToPrefectAdditionalData(int appId, EditLoadDto editData)
        => Log.Quick(() => new EditPrefetchDto
        {
            Links = PrefetchLinks(appId, editData),
            Entities = PrefetchEntities(appId, editData),
            Adam = PrefetchAdam(appId, editData),
        });


    private ICollection<EntityForPickerDto> PrefetchEntities(int appId, EditLoadDto editData)
    {
        var l = Log.Fn<ICollection<EntityForPickerDto>>();
        try
        {
            // Step 1: try to find entity fields
            var bundlesHavingEntities = editData.Items
                // Only these with entity fields
                .Where(b => b.Entity?.Attributes?.Entity?.Any() ?? false)
                .Select(b => new
                {
                    b.Entity!.Guid,
                    b.Entity.Attributes.Entity
                })
                .ToListOpt();

            var entities = bundlesHavingEntities
                .SelectMany(set => set.Entity!
                    .SelectMany(e => e.Value
                        ?.SelectMany(entityAttrib => entityAttrib.Value) ?? []
                    )
                )
                .Where(guid => guid != null)
                .Select(guid => guid.ToString()!)
                // Step 2: Check which ones have a link reference
                .ToListOpt();

            // stop here if nothing found, otherwise the backend will return all entities
            if (!entities.Any())
                return l.Return([], "none found");

            var items = entityPickerBackend.GetForEntityPicker(appId, entities, null, allowFromAllScopes: true);
            return l.Return(items, $"{items.Count}");
        }
        catch
        {
            return l.Return([new() { Id = -1, Text = "Error occurred pre-fetching entities", Value = Guid.Empty }], "error");
        }
    }
}