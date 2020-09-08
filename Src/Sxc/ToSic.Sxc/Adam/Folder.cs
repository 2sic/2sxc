using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Adam
{

    public class Folder : Eav.Apps.Assets.Folder, IFolder
    {
        protected AdamAppContext AdamContext { get; set; }

        public Folder(AdamAppContext adamContext)
        {
            AdamContext = adamContext;
        }

        /// <inheritdoc />
        public dynamic Metadata => Adam.Metadata.GetFirstOrFake(AdamContext, Id, true);

        /// <inheritdoc />
        public bool HasMetadata => Adam.Metadata.GetFirstMetadata(AdamContext.AppRuntime, Id, false) != null;

        /// <inheritdoc />
        public string Url { get; internal set; }

        /// <inheritdoc />
        public string Type => Classification.Folder;


        /// <inheritdoc />
        public override bool HasChildren 
            => _hasChildren ?? (_hasChildren = AdamContext.AdamFs.GetFiles(this).Any() 
                                               || AdamContext.AdamFs.GetFiles(this).Any()).Value;
        private bool? _hasChildren;


        /// <inheritdoc />
        public IEnumerable<IFolder> Folders
        {
            get
            {
                if (_folders != null) return _folders;
                return _folders = string.IsNullOrEmpty(Name)
                    ? new List<Folder>()
                    : AdamContext.AdamFs.GetFolders(this);
            }
        }
        private IEnumerable<IFolder> _folders;


        /// <inheritdoc/>
        public IEnumerable<IFile> Files 
            => _files ?? (_files = AdamContext.AdamFs.GetFiles(this));
        private IEnumerable<IFile> _files;
    }
}