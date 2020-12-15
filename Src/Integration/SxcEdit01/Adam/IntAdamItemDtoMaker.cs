using ToSic.Sxc.WebApi.Adam;

namespace IntegrationSamples.SxcEdit01.Adam
{
    public class IntAdamItemDtoMaker<TFolderId, TFileId> : AdamItemDtoMaker<TFolderId, TFileId>
    {
        #region Constructor / DI

        public IntAdamItemDtoMaker(SecurityChecksBase security): base(security) { }


        #endregion

        public override AdamItemDto Create(ToSic.Sxc.Adam.File<TFolderId, TFileId> original, AdamState state)
        {
            var item = base.Create(original, state);
            if (item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = typed.Id.ToString().Replace("wwwroot\\", ""); // string.Format("todo/{0}/todo/{1}", state.Context.Site.Id, typed.Id);

            return item;
        }


        public override AdamItemDto Create(ToSic.Sxc.Adam.Folder<TFolderId, TFileId> folder, AdamState state)
        {
            var item = base.Create(folder, state);
            if (item is AdamItemDto<TFolderId, TFolderId> typed)
                item.Path = "/" + state.Context.Site.Id + "/api/file/download/" + typed.Id;
            return item;
        }

    }
}
