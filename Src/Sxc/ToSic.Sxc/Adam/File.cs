using System;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.SexyContent.Adam;

namespace ToSic.Sxc.Adam
{
    public class File<TFolderId, TFileId> : Eav.Apps.Assets.File<TFolderId, TFileId>,
#pragma warning disable 618
        AdamFile, 
#pragma warning restore 618
        IFile
    {
        private AdamManager AdamManager { get; }

        public File(AdamManager adamManager)
        {
            AdamManager = adamManager;
        }

        #region Metadata

        /// <inheritdoc />
        public dynamic Metadata => AdamManager.MetadataMaker.GetFirstOrFake(AdamManager, MetadataId);

        public bool HasMetadata => AdamManager.MetadataMaker.GetFirstMetadata(AdamManager.AppRuntime, MetadataId) != null;

        public MetadataFor MetadataId => _metadataKey ?? (_metadataKey = new MetadataFor
        {
            TargetType = (int)TargetTypes.CmsItem,
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