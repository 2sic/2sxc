using System;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Metadata;
using ToSic.SexyContent.Adam;
using ToSic.Sxc.Data;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Adam
{
    public class File<TFolderId, TFileId> : Eav.Apps.Assets.File<TFolderId, TFileId>,
#pragma warning disable 618
        AdamFile, 
#pragma warning restore 618
        IFile
    {
        public File(AdamManager adamManager) => AdamManager = adamManager;
        private AdamManager AdamManager { get; }

        #region Metadata

        /// <inheritdoc />
        [JsonIgnore]
        public IDynamicMetadata Metadata => _metadata ?? (_metadata = AdamManager.MetadataMaker.GetMetadata(AdamManager, CmsMetadata.FilePrefix + SysId, FileName));
        private IDynamicMetadata _metadata;

        /// <inheritdoc />
        [JsonIgnore]
        public bool HasMetadata => (Metadata as IHasMetadata)?.Metadata.Any() ?? false;

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