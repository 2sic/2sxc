using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    [PrivateApi("hide implementation")]
    internal class AdamService: IAdamService, INeedsDynamicCodeRoot
    {
        #region Constructor etc.

        [PrivateApi]        
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _codeRoot = codeRoot;
        private IDynamicCodeRoot _codeRoot;

        #endregion

        /// <inheritdoc />
        public IFile File(int id)
        {
            var admManager = (_codeRoot as DynamicCodeRoot)?.Cdf.AdamManager;
            return admManager?.File(id);
        }

        /// <inheritdoc />
        public IFile File(string id)
        {
            var fileId = AdamManager.CheckIdStringForId(id);
            return fileId == null ? null : File(fileId.Value);
            //if (!id.HasValue()) return null;
            //var linkParts = new LinkParts(id);
            //if (!linkParts.IsMatch || linkParts.Id == 0) return null;
            //return File(linkParts.Id);
        }


        /// <inheritdoc />
        public IFile File(IField field) => field?.Raw is string id ? File(id) : null; // File(field?.Raw as string);

        /// <inheritdoc />
        public IFolder Folder(int id)
        {
            var admManager = (_codeRoot as DynamicCodeRoot)?.Cdf.AdamManager;
            return admManager?.Folder(id);
        }

        /// <inheritdoc />
        public IFolder Folder(IField field) => _codeRoot?.AsAdam(field.Parent, field.Name);
    }
}
