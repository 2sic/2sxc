using ToSic.Eav.Data.Sys.Entities;

namespace ToSic.Sxc.Backend.SaveHelpers;

/// <summary>
/// Extension Methods to Init with slightly different parameters
/// </summary>
internal class SavePermissionDataHelper(ILog parentLog): HelperBase(parentLog, "Sav.TypExt")
{
    /// <summary>
    /// Important: this can only run after init, because AppState isn't available before
    /// </summary>
    internal List<string> ExtractTypeNamesFromItems(IAppReader parent, IEnumerable<ItemIdentifier> items)
    {
        var l = Log.Fn<List<string>>();
        var allData = parent.List;

        l.A($"items in full list: {allData.Count}");

        // build list of type names
        var typeNames = items
            .Select(item =>
            {
                var itemTypeName = item.ContentTypeName;
                var useItemTypeName = !string.IsNullOrEmpty(itemTypeName) || item.EntityId == 0;
                l.A($"item: {item.EntityId}, useItemTypeName: {useItemTypeName}, itemTypeName: {itemTypeName}");

                if (useItemTypeName)
                    return itemTypeName!;

                var entityFromRepo = allData.FindRepoId(item.EntityId)!;
                //if (entityFromRepo == null)
                //{
                //    l.A($"Entity not found in repo: {item.EntityId}");

                //    var entityFromId = allData.One(item.EntityId);

                //    if (entityFromId == null)
                //    {
                //        l.A($"Entity not found in data: {item.EntityId}");
                //    }

                //    throw new ArgumentException($"Can't find this entity so can't find the content-type name");
                //}
                return entityFromRepo.Type.NameId;
            })
            .ToList();

        return l.Return(typeNames, l.Try(() => string.Join(",", typeNames)));
    }

}