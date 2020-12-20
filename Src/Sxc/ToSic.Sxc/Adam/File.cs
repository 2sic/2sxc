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
        private AdamManager AdamContext { get; }

        public File(AdamManager adamContext)
        {
            AdamContext = adamContext;
        }

        #region Metadata

        /// <inheritdoc />
        public dynamic Metadata => AdamContext.MetadataMaker.GetFirstOrFake(AdamContext, MetadataId);

        public bool HasMetadata => AdamContext.MetadataMaker.GetFirstMetadata(AdamContext.AppRuntime, MetadataId) != null;

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