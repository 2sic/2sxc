using System.Collections.Generic;

namespace ToSic.SexyContent.Adam
{
    public class AdamFolder : FolderInfo, IAdamItem
    {

        public AdamBrowseContext AdamBrowseContext;
        public AdamManager Manager;

        private readonly DnnFileSystem dnnfs = new DnnFileSystem();

        /// <summary>
        /// Metadata for this folder
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public DynamicEntity Metadata => AdamBrowseContext.GetFirstMetadata(FolderID, true);

        public bool HasMetadata => AdamBrowseContext.GetFirstMetadataEntity(FolderID, false) != null;

        public string Url => AdamBrowseContext.GenerateWebPath(this);

        public string Type => "folder";

        public string Name { get; internal set; }


        private IEnumerable<AdamFolder> _folders;

        /// <summary>
        ///  Get all subfolders
        /// </summary>
        public IEnumerable<AdamFolder> Folders
        {
            get
            {
                if (_folders != null) return _folders;

                // this is to skip it if it doesn't have subfolders...
                if (!HasChildren || string.IsNullOrEmpty(FolderName))
                    return _folders = new List<AdamFolder>();
                
                _folders = dnnfs.GetFolders(FolderID, AdamBrowseContext);
                return _folders;
            }
        }


        private IEnumerable<AdamFile> _files;

        /// <summary>
        /// Get all files in this folder
        /// </summary>
        public IEnumerable<AdamFile> Files 
            => _files ?? (_files = dnnfs.GetFiles(FolderID, AdamBrowseContext));
    }
}