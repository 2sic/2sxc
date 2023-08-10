using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Adam
{

    public class Folder<TFolderId, TFileId> : Eav.Apps.Assets.Folder<TFolderId, TFileId>, IFolder
    {
        public Folder(AdamManager<TFolderId, TFileId> adamManager) => AdamManager = adamManager;

        protected AdamManager<TFolderId, TFileId> AdamManager { get; }

        /// <inheritdoc />
        [JsonIgnore]
        public IMetadata Metadata => _metadata ?? (_metadata = AdamMetadataMaker.Create(AdamManager, CmsMetadata.FolderPrefix + SysId, Name));
        private IMetadata _metadata;

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



        /// <inheritdoc />
        public IEnumerable<IFolder> Folders => _folders.Get(() =>
        {
            var folders = AdamManager.AdamFs.GetFolders(this);
            folders?.ForEach(f => f.Field = Field);
            return folders;
        });
        private readonly GetOnce<IEnumerable<IFolder>> _folders = new GetOnce<IEnumerable<IFolder>>();


        /// <inheritdoc/>
        public IEnumerable<IFile> Files => _files.Get(() =>
        {
            var files = AdamManager.AdamFs.GetFiles(this);
            files?.ForEach(f => f.Field = Field);
            return files;
        });
        private readonly GetOnce<IEnumerable<IFile>> _files = new GetOnce<IEnumerable<IFile>>();

        IMetadataOf IHasMetadata.Metadata => ((Metadata as ITypedItem)?.Metadata as IHasMetadata)?.Metadata;

        //IMetadataOf IHasMetadata.Metadata
        //    => _metadataOf ?? (_metadataOf = AdamManager.AppContext.AppState.GetMetadataOf(TargetTypes.CmsItem,
        //        CmsMetadata.FolderPrefix + SysId, Name));
        //private IMetadataOf _metadataOf;

        [PrivateApi]
        public IField Field { get; set; }
    }
}