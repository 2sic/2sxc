using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public class OqtAdamItemDtoMaker<TFolderId, TFileId> : AdamItemDtoMaker<TFolderId, TFileId>
    {
        #region Constructor / DI

        public OqtAdamItemDtoMaker(Dependencies dependencies): base(dependencies) { }


        #endregion

        public override AdamItemDto Create(Sxc.Adam.File<TFolderId, TFileId> original)
        {
            var item = base.Create(original);
            if(item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = string.Format(OqtConstants.DownloadLinkTemplate, AdamContext.Context.Site.Id, typed.Id);

            return item;
        }


        public override AdamItemDto Create(Sxc.Adam.Folder<TFolderId, TFileId> folder)
        {
            var item = base.Create(folder);
            if (item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = "/" + AdamContext.Context.Site.Id + "/api/file/download/" + typed.Id;
            return item;
        }

    }
}
