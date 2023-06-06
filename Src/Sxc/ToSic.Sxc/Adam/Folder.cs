using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Adam
{

    public class Folder<TFolderId, TFileId> : Eav.Apps.Assets.Folder<TFolderId, TFileId>, IFolder, ITypedFolder
    {
        public Folder(AdamManager<TFolderId, TFileId> adamManager) => AdamManager = adamManager;

        protected AdamManager<TFolderId, TFileId> AdamManager { get; }

        /// <inheritdoc />
        [JsonIgnore]
        public IDynamicMetadata Metadata => _metadata ?? (_metadata = AdamManager.MetadataMaker.GetDynamic(AdamManager, CmsMetadata.FolderPrefix + SysId, Name));
        private IDynamicMetadata _metadata;

        [JsonIgnore]
        ITypedMetadata IHasMetadata<ITypedMetadata>.Metadata => _typedMd ?? (_typedMd = new TypedMetadata(Metadata, AdamManager.TypedItemHelpers));
        private ITypedMetadata _typedMd;

        /// <inheritdoc />
        [JsonIgnore]
        public bool HasMetadata => (Metadata as IHasMetadata)?.Metadata.Any() ?? false;


        /// <inheritdoc />
        public string Url { get; set; }

        /// <inheritdoc />
        public string Type => Classification.Folder;


        /// <inheritdoc />
        public override bool HasChildren 
            => _hasChildren ?? (_hasChildren = AdamManager.AdamFs.GetFiles(this).Any() 
                                               || AdamManager.AdamFs.GetFolders(this).Any()).Value;
        private bool? _hasChildren;


        [JsonIgnore]
        IEnumerable<ITypedFile> ITypedFolder.Files => Files.Cast<ITypedFile>();

        [JsonIgnore]
        IEnumerable<ITypedFolder> ITypedFolder.Folders => Folders.Cast<ITypedFolder>();

        /// <inheritdoc />
        public IEnumerable<IFolder> Folders => _folders ?? (_folders = AdamManager.AdamFs.GetFolders(this)); 

        private IEnumerable<IFolder> _folders;


        /// <inheritdoc/>
        public IEnumerable<IFile> Files 
            => _files ?? (_files = AdamManager.AdamFs.GetFiles(this));
        private IEnumerable<IFile> _files;


        IMetadataOf IHasMetadata.Metadata
            => _metadataOf ?? (_metadataOf = AdamManager.AppContext.AppState.GetMetadataOf(TargetTypes.CmsItem,
                CmsMetadata.FolderPrefix + SysId, Name));
        private IMetadataOf _metadataOf;
    }
}