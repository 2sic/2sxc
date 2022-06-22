using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services
{
    public class AdamService: IAdamService, INeedsDynamicCodeRoot
    {
        #region Constructor etc.

        
        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => _codeRoot = codeRoot;
        private IDynamicCodeRoot _codeRoot;

        #endregion

        public IFile File(int id)
        {
            var admManager = (_codeRoot as DynamicCodeRoot)?.AdamManager;
            return admManager?.File(id);
        }

        public IFile File(string id)
        {
            if (!id.HasValue()) return null;
            var linkParts = new LinkParts(id);
            if (!linkParts.IsMatch || linkParts.Id == 0) return null;
            return File(linkParts.Id);
        }

        public IFile File(DynamicField field) => File(field?.Raw as string);
    }
}
