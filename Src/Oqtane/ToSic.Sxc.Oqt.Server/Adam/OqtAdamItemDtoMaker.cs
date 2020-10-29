using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public class OqtAdamItemDtoMaker<TFolderId, TFileId> : AdamItemDtoMaker<TFolderId, TFileId>
    {
        #region Constructor / DI

        public OqtAdamItemDtoMaker(SecurityChecksBase security): base(security) { }


        #endregion

        internal override AdamItemDto Create(Sxc.Adam.File<TFolderId, TFileId> original, AdamState state)
        {
            var item = base.Create(original, state);
            if(item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = string.Format(OqtConstants.DownloadLinkTemplate, state.Block.Context.Tenant.Id, typed.Id);

            return item;
        }


        internal override AdamItemDto Create(Sxc.Adam.Folder<TFolderId, TFileId> folder, AdamState state)
        {
            var item = base.Create(folder, state);
            if (item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = "/" + state.Block.Context.Tenant.Id + "/api/file/download/" + typed.Id;
            return item;
        }

    }
}
