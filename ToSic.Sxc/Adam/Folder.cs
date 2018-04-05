using System.Collections.Generic;
using ToSic.Eav.Apps.Assets;
using ToSic.SexyContent;

namespace ToSic.Sxc.Adam
{

    public class Folder : Eav.Apps.Assets.Folder, IAsset
    {
        protected AdamAppContext AppContext { get; set; }

        private readonly IEnvironmentFileSystem _fileSystem;

        public Folder(AdamAppContext appContext, IEnvironmentFileSystem fileSystem)
        {
            AppContext = appContext;
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public DynamicEntity Metadata => Adam.Metadata.GetFirstOrFake(AppContext, Id, true);

        /// <inheritdoc />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AppContext.App, Id, false) != null;

        /// <inheritdoc />
        public string Url => AppContext.Tenant.ContentPath + Path;

        /// <inheritdoc />
       public string Type => Classification.Folder;


        /// <summary>
        ///  Get all subfolders
        /// </summary>
        public IEnumerable<Folder> Folders
        {
            get
            {
                if (_folders != null) return _folders;

                // this is to skip it if it doesn't have subfolders...
                if (!HasChildren || string.IsNullOrEmpty(Name))
                    return _folders = new List<Folder>();
                
                _folders = _fileSystem.GetFolders(Id, AppContext);
                return _folders;
            }
        }
        private IEnumerable<Folder> _folders;




        /// <summary>
        /// Get all files in this folder
        /// </summary>
        public IEnumerable<File> Files 
            => _files ?? (_files = _fileSystem.GetFiles(Id, AppContext));
        private IEnumerable<File> _files;
    }
}