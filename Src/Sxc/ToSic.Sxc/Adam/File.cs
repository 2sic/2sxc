using System;
using System.Linq;
using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Adam
{
    public class File<TFolderId, TFileId> : Eav.Apps.Assets.File<TFolderId, TFileId>,
#pragma warning disable 618
        ToSic.SexyContent.Adam.AdamFile, 
#pragma warning restore 618
        IFile
    {
        public File(AdamManager adamManager) => AdamManager = adamManager;
        private AdamManager AdamManager { get; }

        #region Metadata

        /// <inheritdoc />
        [JsonIgnore]
        public IMetadata Metadata => _metadata 
            ?? (_metadata = AdamManager.MetadataMaker.Get(AdamManager, CmsMetadata.FilePrefix + SysId, FileName, AttachMdRecommendations));
        private IMetadata _metadata;

        /// <summary>
        /// Attach metadata recommendations
        /// </summary>
        /// <param name="mdOf"></param>
        private void AttachMdRecommendations(IMetadataOf mdOf)
        {
            if (mdOf?.Target == null) return;
            if (Type == Classification.Image)
                mdOf.Target.Recommendations = new[] { ImageDecorator.TypeNameId };
        }

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
            => _metadataOf ?? (_metadataOf = AdamManager.AppContext.AppState.GetMetadataOf(TargetTypes.CmsItem,
                CmsMetadata.FilePrefix + SysId, FileName));
        private IMetadataOf _metadataOf;
    }
}