using System.Collections.Generic;
using System.Linq;

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
        public dynamic Metadata => Adam.Metadata.GetFirstOrFake(AppContext, Id, true);

        /// <inheritdoc />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AppContext.AppRuntime, Id, false) != null;

        /// <inheritdoc />
        public string Url => AppContext.Tenant.ContentPath + Path;

        /// <inheritdoc />
        public string Type => Classification.Folder;


        /// <inheritdoc />
        public override bool HasChildren => _hasChildren ?? (_hasChildren = _fileSystem.GetFiles(Id, AppContext).Any() || _fileSystem.GetFiles(Id, AppContext).Any()).Value;
        private bool? _hasChildren;


        /// <inheritdoc />
        public IEnumerable<IFolder> Folders
        {
            get
            {
                if (_folders != null) return _folders;
                return _folders = string.IsNullOrEmpty(Name)
                    ? new List<Folder>()
                    : _fileSystem.GetFolders(Id, AppContext);
            }
        }
        private IEnumerable<IFolder> _folders;


        /// <inheritdoc/>
        public IEnumerable<IFile> Files 
            => _files ?? (_files = _fileSystem.GetFiles(Id, AppContext));
        private IEnumerable<IFile> _files;
    }
}