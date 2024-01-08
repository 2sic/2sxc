using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Oqt.Server.Adam;

// TODO: @STV - this doesn't seem to be used, as it's not even registered in DI
// Pls find out why, and if we don't need it, remove
internal class OqtAdamItemDtoMaker<TFolderId, TFileId> : AdamItemDtoMaker<TFolderId, TFileId>
{
    #region Constructor / DI

    public OqtAdamItemDtoMaker(MyServices services): base(services) { }


    #endregion

    public override AdamItemDto Create(File<TFolderId, TFileId> original)
    {
        var item = base.Create(original);
        if(item is AdamItemDto<TFolderId, TFolderId> typed)
            item.Path = string.Format(OqtConstants.DownloadLinkTemplate, AdamContext.Context.Site.Id, typed.Id);

        return item;
    }


    public override AdamItemDto Create(Folder<TFolderId, TFileId> folder)
    {
        var item = base.Create(folder);
        if (item is AdamItemDto<TFolderId, TFolderId> typed)
            item.Path = "/" + AdamContext.Context.Site.Id + "/api/file/download/" + typed.Id;
        return item;
    }

}