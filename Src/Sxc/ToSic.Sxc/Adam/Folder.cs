using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Adam
{

    public class Folder<TFolderId, TFileId> : Eav.Apps.Assets.Folder<TFolderId, TFileId>, IFolder
    {
        protected AdamManager<TFolderId, TFileId> AdamManager { get; set; }

        public Folder(AdamManager<TFolderId, TFileId> adamManager)
        {
            AdamManager = adamManager;
        }

        /// <inheritdoc />
        [JsonIgnore]
        public dynamic Metadata => AdamManager.MetadataMaker.GetFirstOrFake(AdamManager, MetadataId);

        /// <inheritdoc />
        [JsonIgnore]
        public bool HasMetadata => AdamManager.MetadataMaker.GetFirstMetadata(AdamManager.AppRuntime, MetadataId) != null;

        [JsonIgnore]
        public ITarget MetadataId => _metadataId ?? (_metadataId = new Target
        {
            TargetType = (int)TargetTypes.CmsItem,
            KeyString = CmsMetadata.FolderPrefix /*"folder:"*/ + SysId
        });
        private ITarget _metadataId;

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
        public IEnumerable<IFolder> Folders => _folders ?? (_folders = AdamManager.AdamFs.GetFolders(this)); 

        private IEnumerable<IFolder> _folders;


        /// <inheritdoc/>
        public IEnumerable<IFile> Files 
            => _files ?? (_files = AdamManager.AdamFs.GetFiles(this));
        private IEnumerable<IFile> _files;
    }
}