using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Adam
{

    public class Folder : Eav.Apps.Assets.Folder, IFolder
    {
        protected AdamAppContext AppContext { get; set; }

        private readonly IEnvironmentFileSystem _fileSystem;

        public Folder(AdamAppContext appContext, IEnvironmentFileSystem fileSystem)
        {
            AppContext = appContext;
            _fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public dynamic Metadata => Adam.Metadata.GetFirstOrFake(AppContext, Id, true) as dynamic;

        /// <inheritdoc />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AppContext.AppRuntime, Id, false) != null;

        /// <inheritdoc />
        public string Url => AppContext.Tenant.ContentPath + Path;

        /// <inheritdoc />
        public string Type => Classification.Folder;

        [PrivateApi]
        [Obsolete]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int FileId => throw new NotImplementedException();

        [PrivateApi]
        [Obsolete]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public string FileName => throw new NotImplementedException();


        /// <inheritdoc />
        public IEnumerable<IFolder> Folders
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
        private IEnumerable<IFolder> _folders;




        /// <inheritdoc/>
        public IEnumerable<Sxc.Adam.IFile> Files 
            => _files ?? (_files = _fileSystem.GetFiles(Id, AppContext));
        private IEnumerable<Sxc.Adam.IFile> _files;
    }
}