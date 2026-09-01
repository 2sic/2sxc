using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sxc.Adam.Sys.Work;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class EditLoadActivityAddPrefetch(
    Generator<HyperlinkBackend> hyperlinkBackend,
    Generator<IAdamPrefetchHelper, AdamWorkOptions> adamTransGetItems,
    EntityPickerApi entityPickerBackend)
    : ServiceBase(SxcLogName + ".Prefetch", connect: [adamTransGetItems, hyperlinkBackend, entityPickerBackend]),
        IWork<EditLoadDto, EditLoadDto>
{
    public async Task<Package<EditLoadDto>> Handle(WorkContext actionCtx, Package<EditLoadDto> package)
    {
        var l = Log.Fn<Package<EditLoadDto>>();
        try
        {
            package = package with
            {
                Data = package.Data with
                {
                    Prefetch = TryToPrefectAdditionalData(actionCtx.Get<int>(EditLoadContextConstants.AppId), package.Data)
                }
            };
            return l.Return(package, "prefetched");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(package, "error prefetching");
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