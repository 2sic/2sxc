using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    [PrivateApi("hide implementation")]
    public class AdamService: IAdamService, INeedsDynamicCodeRoot
    {
        #region Constructor etc.

        [PrivateApi]        
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _codeRoot = codeRoot;
        private IDynamicCodeRoot _codeRoot;

        #endregion

        /// <inheritdoc />
        public IFile File(int id)
        {
            var admManager = (_codeRoot as DynamicCodeRoot)?.AdamManager;
            return admManager?.File(id);
        }

        /// <inheritdoc />
        public IFile File(string id)
        {
            if (!id.HasValue()) return null;
            var linkParts = new LinkParts(id);
            if (!linkParts.IsMatch || linkParts.Id == 0) return null;
            return File(linkParts.Id);
        }

        /// <inheritdoc />
        public IFile File(IDynamicField field) => File(field?.Raw as string);

        /// <inheritdoc />
        public IFolder Folder(IDynamicField field) => _codeRoot?.AsAdam(field.Parent, field.Name);
    }
}
