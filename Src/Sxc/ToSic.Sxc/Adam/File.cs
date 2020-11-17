using System;
using ToSic.Eav.Data;
using ToSic.SexyContent.Adam;

namespace ToSic.Sxc.Adam
{
    public class File<TFolderId, TFileId> : Eav.Apps.Assets.File<TFolderId, TFileId>,
#pragma warning disable 618
        AdamFile, 
#pragma warning restore 618
        IFile
    {
        private AdamAppContext AppContext { get; }

        public File(AdamAppContext appContext)
        {
            AppContext = appContext;
        }

        #region Metadata

        /// <inheritdoc />
        public dynamic Metadata => Adam.Metadata.GetFirstOrFake(AppContext, MetadataId);

        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AppContext.AppRuntime, MetadataId) != null;

        public MetadataFor MetadataId => _metadataKey ?? (_metadataKey = new MetadataFor
        {
            TargetType = Eav.Constants.MetadataForCmsObject,
            KeyString = "file:" + SysId
        });
        private MetadataFor _metadataKey;
        #endregion

        public string Url { get; set; }

        public string Type => Classification.TypeName(Extension);



        public string FileName => FullName;

        public DateTime CreatedOnDate => Created;

        public int FileId => SysId as int? ?? 0;

    }
}