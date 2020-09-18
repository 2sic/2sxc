using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Adam
{

    public class Folder<TFolderId, TFileId> : Eav.Apps.Assets.Folder<TFolderId, TFileId>, IFolder
    {
        protected AdamAppContext<TFolderId, TFileId> AdamContext { get; set; }

        public Folder(AdamAppContext<TFolderId, TFileId> adamContext)
        {
            AdamContext = adamContext;
        }

        /// <inheritdoc />
        public dynamic Metadata => Adam.Metadata.GetFirstOrFake(AdamContext, MetadataId);

        /// <inheritdoc />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AdamContext.AppRuntime, MetadataId) != null;

        public MetadataFor MetadataId => _metadataKey ?? (_metadataKey = new MetadataFor
        {
            TargetType = Eav.Constants.MetadataForCmsObject,
            KeyString = "folder:" + SysId
        });
        private MetadataFor _metadataKey;

        /// <inheritdoc />
        public string Url { get; internal set; }

        /// <inheritdoc />
        public string Type => Classification.Folder;


        /// <inheritdoc />
        public override bool HasChildren 
            => _hasChildren ?? (_hasChildren = AdamContext.AdamFs.GetFiles(this).Any() 
                                               || AdamContext.AdamFs.GetFolders(this).Any()).Value;
        private bool? _hasChildren;


        /// <inheritdoc />
        public IEnumerable<IFolder> Folders => _folders ?? (_folders = AdamContext.AdamFs.GetFolders(this)); 

        private IEnumerable<IFolder> _folders;


        /// <inheritdoc/>
        public IEnumerable<IFile> Files 
            => _files ?? (_files = AdamContext.AdamFs.GetFiles(this));
        private IEnumerable<IFile> _files;
    }
}