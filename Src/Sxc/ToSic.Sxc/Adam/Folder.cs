using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Adam
{

    public class Folder : Eav.Apps.Assets.Folder, IFolder
    {
        protected AdamAppContext AdamContext { get; set; }

        //private readonly IEnvironmentFileSystem _fileSystem;

        public Folder(AdamAppContext adamContext/*, IEnvironmentFileSystem fileSystem*/)
        {
            AdamContext = adamContext;
            //_fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public dynamic Metadata => Adam.Metadata.GetFirstOrFake(AdamContext, Id, true);

        /// <inheritdoc />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AdamContext.AppRuntime, Id, false) != null;

        /// <inheritdoc />
        public string Url { get; internal set; }  // AdamContext.Tenant.ContentPath + Path;

        /// <inheritdoc />
        public string Type => Classification.Folder;


        /// <inheritdoc />
        public override bool HasChildren 
            => _hasChildren ?? (_hasChildren = AdamContext.EnvironmentFs.GetFiles(Id/*, AppContext*/).Any() 
                                               || AdamContext.EnvironmentFs.GetFiles(Id/*, AppContext*/).Any()).Value;
        private bool? _hasChildren;


        /// <inheritdoc />
        public IEnumerable<IFolder> Folders
        {
            get
            {
                if (_folders != null) return _folders;
                return _folders = string.IsNullOrEmpty(Name)
                    ? new List<Folder>()
                    : AdamContext.EnvironmentFs.GetFolders(Id/*, AppContext*/);
            }
        }
        private IEnumerable<IFolder> _folders;


        /// <inheritdoc/>
        public IEnumerable<IFile> Files 
            => _files ?? (_files = AdamContext.EnvironmentFs.GetFiles(Id/*, AppContext*/));
        private IEnumerable<IFile> _files;
    }
}