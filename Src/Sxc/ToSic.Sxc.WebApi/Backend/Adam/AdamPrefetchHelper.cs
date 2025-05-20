using ToSic.Sxc.Adam.Work.Internal;

namespace ToSic.Sxc.Backend.Adam;

/// <summary>
/// Backend for the API
/// Is meant to be transaction based - so create a new one for each thing as the initializers set everything for the transaction
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamPrefetchHelper(Generator<AdamWorkGet, AdamWorkOptions> adamGet, Generator<IAdamItemDtoMaker, AdamItemDtoMakerOptions> dtoMaker)
    : ServiceWithSetup<AdamWorkOptions>("Adm.TrnItm"),
        IAdamPrefetchHelper
{
    public IList<AdamItemDto> GetAdamItemsForPrefetch(string subFolderName, bool autoCreate = true)
    {
        var adamGetReady = adamGet.New(Options);
        var items = adamGetReady.ItemsInField(subFolderName, autoCreate);
        var maker = dtoMaker.New(new() { AdamContext = adamGetReady.AdamContext, });
        return maker.Convert(items).ToList();
    }
}