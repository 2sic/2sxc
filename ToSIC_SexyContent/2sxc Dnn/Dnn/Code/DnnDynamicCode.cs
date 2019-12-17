using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.Sxc.Dnn.Code
{
    public class DnnDynamicCode : Sxc.Code.DynamicCodeBase, IDnnDynamicCode
    {
        public ICmsBlock CmsBlock;

        public DnnDynamicCode(ICmsBlock cmsBlock, ILog parentLog = null): base(cmsBlock, new DnnTenant(null), parentLog)
        {
            CmsBlock = cmsBlock;
            // Init things than require module-info or similar, but not 2sxc
            var instance = cmsBlock?.Container;
            Dnn = new DnnContext(instance);
            Link = new DnnLinkHelper(Dnn);
        }

        #region IHasDnnContext

        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public IDnnContext Dnn { get; }

        #endregion


    }
}