namespace ToSic.Sxc.Backend.Adam;

/// <summary>
/// Backend for the API
/// Is meant to be transaction based - so create a new one for each thing as the initializers set everything for the transaction
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamPrefetchHelper<TFolderId, TFileId>(AdamWorkBase<TFolderId, TFileId>.MyServices services, AdamItemDtoMaker<TFolderId, TFileId> dtoMaker)
    : AdamWorkBase<TFolderId, TFileId>(services, "Adm.TrnItm"),
        IAdamPrefetchHelper
{
    public IList<AdamItemDto> GetAdamItemsForPrefetch(string subFolderName, bool autoCreate = true)
    {
        var items = ItemsInField(subFolderName, autoCreate);
        var maker = dtoMaker.SpawnNew(new() { AdamContext = AdamContext, });
        return maker.Convert(items).ToList();
    }
}