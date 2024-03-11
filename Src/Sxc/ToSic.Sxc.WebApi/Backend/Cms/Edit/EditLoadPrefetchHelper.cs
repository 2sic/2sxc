using ToSic.Eav.WebApi;
using ToSic.Sxc.Backend.Adam;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class EditLoadPrefetchHelper(
    Generator<HyperlinkBackend<int, int>> hyperlinkBackend,
    Generator<IAdamTransGetItems> adamTransGetItems,
    EntityPickerApi entityPickerBackend)
    : ServiceBase(SxcLogName + ".Prefetch", connect: [adamTransGetItems, hyperlinkBackend, entityPickerBackend])
{
    public EditPrefetchDto TryToPrefectAdditionalData(int appId, EditDto editData) => Log.Func(() =>
        new EditPrefetchDto
        {
            Links = PrefetchLinks(appId, editData),
            Entities = PrefetchEntities(appId, editData),
            Adam = PrefetchAdam(appId, editData),
        });


    private List<EntityForPickerDto> PrefetchEntities(int appId, EditDto editData)
    {
        var l = Log.Fn<List<EntityForPickerDto>>();
        try
        {
            // Step 1: try to find entity fields
            var bundlesHavingEntities = editData.Items
                // Only these with entity fields
                .Where(b => b.Entity?.Attributes?.Entity?.Any() ?? false)
                .Select(b => new
                {
                    b.Entity.Guid,
                    b.Entity.Attributes.Entity
                })
                .ToList();

            var entities = bundlesHavingEntities.SelectMany(set
                    => set.Entity.SelectMany(e
                        => e.Value?.SelectMany(entityAttrib => entityAttrib.Value)))
                .Where(guid => guid != null)
                .Select(guid => guid.ToString())
                // Step 2: Check which ones have a link reference
                .ToArray();

            // stop here if nothing found, otherwise the backend will return all entities
            if (!entities.Any()) return l.Return([], "none found");

            var items = entityPickerBackend.GetForEntityPicker(appId, entities, null, allowFromAllScopes: true);
            return l.Return(items, $"{items.Count}");
        }
        catch
        {
            return l.Return([new() { Id = -1, Text = "Error occurred pre-fetching entities", Value = Guid.Empty }], "error");
        }
    }
}