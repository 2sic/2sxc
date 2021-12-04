using System;
using Newtonsoft.Json;
using ToSic.Eav.Metadata;
using ToSic.SexyContent.Adam;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

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
        [JsonIgnore]
        public dynamic Metadata => _metadata ?? (_metadata = AdamManager.MetadataMaker.GetFirstOrFake(AdamManager, MetadataId));
        private dynamic _metadata;
        // TODO: PROBABLY CHANGE these Hasmetadata etc. to just use the new IHasMetadata.Metadata property to start with
        [JsonIgnore] public bool HasMetadata => AdamManager.MetadataMaker.GetFirstMetadata(AdamManager.AppRuntime, MetadataId) != null;

        [JsonIgnore]
        private ITarget MetadataId => _metadataId ?? (_metadataId = new Target((int)TargetTypes.CmsItem, FileName) { KeyString = CmsMetadata.FilePrefix + SysId });
        private ITarget _metadataId;

        #endregion

        public string Url { get; set; }

        public string Type => Classification.TypeName(Extension);



        public string FileName => FullName;

        public DateTime CreatedOnDate => Created;

        public int FileId => SysId as int? ?? 0;

        IMetadataOf IHasMetadata.Metadata
            => _metadataOf ?? (_metadataOf = AdamManager.AppContext.AppState.GetMetadataOf((int)TargetTypes.CmsItem,
                CmsMetadata.FilePrefix + SysId, FileName));
        private IMetadataOf _metadataOf;
    }
}