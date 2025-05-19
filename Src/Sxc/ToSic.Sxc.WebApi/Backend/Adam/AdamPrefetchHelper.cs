using ToSic.Sxc.Adam.Work.Internal;
using ToSic.Sys.Services;

namespace ToSic.Sxc.Backend.Adam;

/// <summary>
/// Backend for the API
/// Is meant to be transaction based - so create a new one for each thing as the initializers set everything for the transaction
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamPrefetchHelper<TFolderId, TFileId>(Generator<IAdamWorkGet, AdamWorkOptions> adamGet, Generator<IAdamItemDtoMaker, AdamItemDtoMakerOptions> dtoMaker)
    : ServiceWithOptionsBaseLightWip<AdamWorkOptions>("Adm.TrnItm"),
        IAdamPrefetchHelper,
        IServiceWithOptionsToSetup<AdamWorkOptions>
{
    public IList<AdamItemDto> GetAdamItemsForPrefetch(string subFolderName, bool autoCreate = true)
    {

        var adamGetReady = adamGet.New(_options);
        var items = adamGetReady.ItemsInField(subFolderName, autoCreate);
        var maker = dtoMaker.New(new() { AdamContext = adamGetReady.AdamContext, });
        return maker.Convert(items).ToList();
    }

    public void SetOptions(AdamWorkOptions options) => _options = options;

    private AdamWorkOptions? _options;
}