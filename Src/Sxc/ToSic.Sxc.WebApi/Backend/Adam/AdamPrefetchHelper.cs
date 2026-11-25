using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Adam.Sys.Work;

namespace ToSic.Sxc.Backend.Adam;

/// <summary>
/// Backend for the API
/// Is meant to be transaction based - so create a new one for each thing as the initializers set everything for the transaction
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamPrefetchHelper(
    Generator<AdamContext, AdamContext.Options> adamContextGen,
    Generator<AdamWorkGet, AdamContext> adamGet,
    Generator<IAdamItemDtoMaker, AdamItemDtoMakerOptions> dtoMaker)
    : ServiceWithSetup<AdamContext.Options>("Adm.TrnItm", connect: [adamContextGen, adamGet, dtoMaker]),
        IAdamPrefetchHelper
{
    public ICollection<AdamItemDto> GetAdamItemsForPrefetch(string subFolderName, bool autoCreate = true)
    {
        var l = Log.Fn<ICollection<AdamItemDto>>($"subFolderName:{subFolderName}, autoCreate:{autoCreate}");
        var adamCtx = adamContextGen.New(MyOptions);
        var adamGetReady = adamGet.New(adamCtx);
        var items = adamGetReady.ItemsInField(subFolderName, autoCreate);
        
        if (items == null)
            return l.ReturnAsError([], "got empty object, user is probably restricted");

        var maker = dtoMaker.New(new() { AdamContext = adamCtx, });
        var result = maker.Convert(items)
            .ToListOpt();
        return l.ReturnAsOk(result);
    }

}
