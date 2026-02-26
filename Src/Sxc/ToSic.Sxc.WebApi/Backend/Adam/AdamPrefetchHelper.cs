using ToSic.Sxc.Adam.Sys.Work;

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
    public ICollection<AdamItemDto> GetAdamItemsForPrefetch(string subFolderName, bool autoCreate = true)
    {
        var l = Log.Fn<ICollection<AdamItemDto>>($"subFolderName:{subFolderName}, autoCreate:{autoCreate}");
        var adamGetReady = adamGet.New(MyOptions);
        var items = adamGetReady.ItemsInField(subFolderName, autoCreate);
        
        if (items == null)
            return l.ReturnAsError([], "got empty object, user is probably restricted");

        var maker = dtoMaker.New(new() { AdamContext = adamGetReady.AdamContext, });
        var result = maker.Convert(items)
            .ToListOpt();
        return l.ReturnAsOk(result);
    }
}