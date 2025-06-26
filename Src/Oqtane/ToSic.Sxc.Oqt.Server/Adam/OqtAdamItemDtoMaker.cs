using ToSic.Sxc.Adam;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam;

// TODO: @STV - this doesn't seem to be used, as it's not even registered in DI
// Pls find out why, and if we don't need it, remove
internal class OqtAdamItemDtoMaker(AdamItemDtoMaker<int, int>.MyServices services)
    : AdamItemDtoMaker<int, int>(services)
{
    public override AdamItemDto Create(IFile original)
    {
        var item = base.Create(original);
        if(item is AdamItemDto<int, int> typed)
            item.Path = string.Format(OqtConstants.DownloadLinkTemplate, AdamContext.Context.Site.Id, typed.Id);

        return item;
    }


    public override AdamItemDto Create(IFolder folder)
    {
        var item = base.Create(folder);
        if (item is AdamItemDto<int, int> typed)
            item.Path = "/" + AdamContext.Context.Site.Id + "/api/file/download/" + typed.Id;
        return item;
    }

}