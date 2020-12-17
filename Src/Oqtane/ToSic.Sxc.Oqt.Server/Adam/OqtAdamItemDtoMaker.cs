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

        public override AdamItemDto Create(Sxc.Adam.File<TFolderId, TFileId> original/*, AdamState state*/)
        {
            var item = base.Create(original/*, state*/);
            if(item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = string.Format(OqtConstants.DownloadLinkTemplate, AdamState.Context.Site.Id, typed.Id);

            return item;
        }


        public override AdamItemDto Create(Sxc.Adam.Folder<TFolderId, TFileId> folder/*, AdamState state*/)
        {
            var item = base.Create(folder/*, state*/);
            if (item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = "/" + AdamState.Context.Site.Id + "/api/file/download/" + typed.Id;
            return item;
        }

    }
}
